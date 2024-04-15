using Product.Service.DTOs.Accounts;
using Product.Service.DTOs.Admins;

namespace Product.Service.Interfaces.Accounts
{
    public interface IAccountService
    {
        Task<bool> AdminRegisterAsync(AdminRegisterDto adminRegisterDto);
        Task<string> LoginAsync(AccountLoginDto accountLoginDto);
    }
}
