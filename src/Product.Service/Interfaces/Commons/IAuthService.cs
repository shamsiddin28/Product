using Product.Domain.Entities;

namespace Product.Service.Interfaces.Commons
{
    public interface IAuthService
    {
        string GenerateToken(Human human, string role);
    }
}
