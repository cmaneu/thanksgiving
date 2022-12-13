using api.ApplicationCore.Entities;
using api.Services;

namespace api.Adapters
{
    public class FakeUserRepository : IUserRepository
    {
        // TODO: Implement this repository
        public Task<User?> GetByEmailAsync(string userEmail)
        {
            if (userEmail == "chris@demo.org")
                return Task.FromResult(new User()
                {
                    Id = 237,
                    Email = "chris@demo.org",
                    FirstName = "Christopher",
                    LastName = "MANEU"
                });

            return Task.FromResult<User>(null);
        }

        public Task<User> GetByIdAsync(string userId)
        {
            return Task.FromResult(new User()
            {
                Id = 237,
                Email = "chris@demo.org",
                FirstName = "Christopher",
                LastName = "MANEU"
            });
        }
    }
}
