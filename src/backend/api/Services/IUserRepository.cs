using api.ApplicationCore.Entities;

namespace api.Services;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string userEmail);
    Task<User> GetByIdAsync(string userId);
}
