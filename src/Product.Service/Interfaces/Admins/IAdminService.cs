using Microsoft.AspNetCore.Http;
using Product.Service.DTOs.Admins;
using Product.Service.ViewModels.AdminViewModels;

namespace Product.Service.Interfaces.Admins
{
    public interface IAdminService
    {
        Task<AdminViewModel> GetByPhoneNumberAsync(string phoneNumber);

        Task<List<AdminViewModel>> GetAllAsync(string search);

        Task<List<AdminViewModel>> GetAllAsync();

        Task<AdminViewModel> GetByIdAsync(long id);

        Task<bool> UpdateAsync(long id, AdminUpdateDto adminUpdatedDto);

        Task<bool> UpdateImageAsync(long id, IFormFile formFile);

        Task<bool> DeleteAsync(long id);

        Task<bool> DeleteImageAsync(long adminId);

        Task<bool> UpdatePasswordAsync(long id, PasswordUpdateDto dto);
    }
}
