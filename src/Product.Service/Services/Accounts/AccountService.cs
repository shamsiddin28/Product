using AutoMapper;
using Product.Data.Interfaces.IRepositories;
using Product.Domain.Entities;
using Product.Domain.Enums;
using Product.Service.Commons.Helpers;
using Product.Service.Commons.Security;
using Product.Service.DTOs.Accounts;
using Product.Service.DTOs.Admins;
using Product.Service.Exceptions;
using Product.Service.Interfaces.Accounts;
using Product.Service.Interfaces.Commons;
using Product.Service.Interfaces.Files;

namespace Product.Service.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Admin> _repository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public AccountService(IRepository<Admin> repository, IAuthService authService, IMapper mapper, IFileService fileService)
        {
            this._repository = repository;
            this._authService = authService;
            this._mapper = mapper;
            this._fileService = fileService;
        }

        public async Task<bool> AdminRegisterAsync(AdminRegisterDto adminRegisterDto)
        {
            var phoneNumberCheck = await _repository.FirstOrDefaultAsync(x => x.PhoneNumber == adminRegisterDto.PhoneNumber);
            if (phoneNumberCheck is not null)
                throw new AlreadyExistingException(nameof(adminRegisterDto.PhoneNumber), "This phone number is already registered.");

            var hashresult = PasswordHasher.Hash(adminRegisterDto.Password);
            var admin = _mapper.Map<Admin>(adminRegisterDto);
            admin.AdminRole = Role.Admin;
            admin.PasswordHash = hashresult.Hash;
            admin.Salt = hashresult.Salt;
            admin.CreatedAt = TimeHelper.GetCurrentServerTime();
            _repository.Insert(admin);
            var result = await _repository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<string> LoginAsync(AccountLoginDto accountLoginDto)
        {
            var admin = await _repository.FirstOrDefaultAsync(x => x.PhoneNumber == accountLoginDto.PhoneNumber);
            if (admin is null)
                throw new NotFoundException(nameof(accountLoginDto.PhoneNumber), "No admin with this phone number is found!");
            else
            {
                var hasherResult = PasswordHasher.Verify(accountLoginDto.Password, admin.Salt, admin.PasswordHash);
                if (hasherResult)
                {
                    string token = "";
                    if (admin.PhoneNumber != null)
                    {
                        if (admin.AdminRole == Role.Admin)
                        {
                            token = _authService.GenerateToken(admin, "admin");
                            return token;
                        }
                        else if (admin.AdminRole == Role.SuperAdmin)
                        {
                            token = _authService.GenerateToken(admin, "superadmin");
                            return token;
                        }
                    }
                    if (admin.AdminRole == Role.Admin)
                    {
                        token = _authService.GenerateToken(admin, "admin");
                        return token;
                    }
                    else if (admin.AdminRole == Role.SuperAdmin)
                    {
                        token = _authService.GenerateToken(admin, "superadmin");
                        return token;
                    }
                    token = _authService.GenerateToken(admin, "admin");
                    return token;
                }
                else throw new NotFoundException(nameof(accountLoginDto.Password), "Incorrect password!");
            }
        }
    }
}
