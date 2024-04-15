using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Product.Data.Interfaces.IRepositories;
using Product.Domain.Entities;
using Product.Service.Commons.Helpers;
using Product.Service.Commons.Security;
using Product.Service.DTOs.Admins;
using Product.Service.Exceptions;
using Product.Service.Interfaces.Admins;
using Product.Service.Interfaces.Commons;
using Product.Service.Interfaces.Files;
using Product.Service.ViewModels.AdminViewModels;

namespace Product.Service.Services.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Admin> _repository;
        private readonly IIdentityService _identityService;
        private readonly IFileService _fileService;

        public AdminService(IRepository<Admin> repository, IIdentityService identityService, IFileService fileService)
        {
            this._repository = repository;
            this._identityService = identityService;
            this._fileService = fileService;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var admin = await _repository.FindByIdAsync(id);
            if (admin is null) throw new NotFoundException("Admin", $"{id} not found");
            _repository.Delete(id);
            int result = await _repository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteImageAsync(long adminId)
        {
            var admin = await _repository.FindByIdAsync(adminId);
            if (admin is null) throw new NotFoundException("Admin", $"{adminId} not found");
            else
            {
                await _fileService.DeleteFileAsync(admin.Image!);
                admin.Image = "";
                _repository.Update(adminId, admin);
                var res = await _repository.SaveChangesAsync();
                return res > 0;
            }
        }

        public async Task<List<AdminViewModel>> GetAllAsync(string search)
        {
            var query = _repository.SelectAll();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.FirstName.ToLower().StartsWith(search.ToLower()) || x.LastName.ToLower().StartsWith(search.ToLower()) || x.Address.ToLower().StartsWith(search.ToLower()));
            }

            var result = await query.OrderByDescending(x => x.CreatedAt).Select(x => (AdminViewModel)x).ToListAsync();
            return result;
        }

        public Task<List<AdminViewModel>> GetAllAsync()
        {
            var query = _repository.SelectAll().OrderByDescending(x => x.CreatedAt).Select(x => (AdminViewModel)x).ToListAsync();
            return query;
        }

        public async Task<AdminViewModel> GetByIdAsync(long id)
        {
            var admin = await _repository.FindByIdAsync(id);
            if (admin is null) throw new NotFoundException("Admin", $"{id} not found");
            var adminView = (AdminViewModel)admin;
            return adminView;
        }

        public async Task<AdminViewModel> GetByPhoneNumberAsync(string phoneNumber)
        {
            var admin = await _repository.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (admin is null) throw new NotFoundException("Admin", $"{phoneNumber} not found");
            var adminView = (AdminViewModel)admin;
            return adminView;
        }

        public async Task<bool> UpdateAsync(long id, AdminUpdateDto adminUpdatedDto)
        {
            var admin = await _repository.FindByIdAsync(id);
            if (admin is null) throw new NotFoundException("Admin", $"{id} not found");
            _repository.TrackingDeteched(admin);
            if (adminUpdatedDto != null)
            {
                admin.FirstName = string.IsNullOrEmpty(adminUpdatedDto.FirstName) ? admin.FirstName : adminUpdatedDto.FirstName;
                admin.LastName = string.IsNullOrEmpty(adminUpdatedDto.LastName) ? admin.LastName : adminUpdatedDto.LastName;
                admin.Image = string.IsNullOrEmpty(adminUpdatedDto.ImagePath) ? admin.Image : adminUpdatedDto.ImagePath;
                admin.PhoneNumber = string.IsNullOrEmpty(adminUpdatedDto.PhoneNumber) ? admin.PhoneNumber : adminUpdatedDto.PhoneNumber;
                admin.BirthDate = admin.BirthDate;
                admin.Address = string.IsNullOrEmpty(adminUpdatedDto.Address) ? admin.Address : adminUpdatedDto.Address;
                if (adminUpdatedDto.Image is not null)
                {
                    admin.Image = await _fileService.UploadImageAsync(adminUpdatedDto.Image);
                }
                admin.UpdatedAt = TimeHelper.GetCurrentServerTime();
                _repository.Update(id, admin);
                var result = await _repository.SaveChangesAsync();
                return result > 0;
            }
            else throw new ModelErrorException("", "Not found");
        }

        public async Task<bool> UpdateImageAsync(long id, IFormFile formFile)
        {
            var admin = await _repository.FindByIdAsync(id);
            var updateImage = await _fileService.UploadImageAsync(formFile);
            var adminUpdatedDto = new AdminUpdateDto()
            {
                ImagePath = updateImage
            };
            var result = await UpdateAsync(id, adminUpdatedDto);
            return result;
        }

        public async Task<bool> UpdatePasswordAsync(long id, PasswordUpdateDto dto)
        {
            var admin = await _repository.FindByIdAsync(id);
            if (admin is null)
                throw new StatusCodeException(System.Net.HttpStatusCode.NotFound, "Admin is not found");
            _repository.TrackingDeteched(admin);
            var res = PasswordHasher.Verify(dto.OldPassword, admin.Salt, admin.PasswordHash);
            if (res)
            {
                if (dto.NewPassword == dto.VerifyPassword)
                {
                    var hash = PasswordHasher.Hash(dto.NewPassword);
                    admin.PasswordHash = hash.Hash;
                    admin.Salt = hash.Salt;
                    _repository.Update(id, admin);
                    var result = await _repository.SaveChangesAsync();
                    return result > 0;
                }
                else throw new StatusCodeException(System.Net.HttpStatusCode.BadRequest, "new password and verify" + " password must be match!");
            }
            else throw new StatusCodeException(System.Net.HttpStatusCode.BadRequest, "Invalid Password");
        }

    }
}
