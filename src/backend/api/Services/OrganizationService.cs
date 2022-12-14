using api.Adapters;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class OrganizationService
    {
        private IOrganizationRepository _orgRepo;

        public OrganizationService(IOrganizationRepository orgRepo)
        {
            _orgRepo = orgRepo;
        }

        public async Task<IEnumerable<Organization>> GetAllPublicOrganizationsAsync()
        {
            return await _orgRepo.GetAllPublicOrganizationsAsync();
        }

        public async Task<IEnumerable<Activity>> GetUserAllActivitiesAsync(string organizationSlug, int userId)
        {
            return await _orgRepo.GetAllUserActivity(organizationSlug, userId);
        }

        internal async Task ValidateAllUserActivitiesAsync(string organizationSlug, int userId)
        {
            await _orgRepo.ValidateAllUserActivitiesAsync(organizationSlug, userId);
        }
    }

    public interface IOrganizationRepository
    {
        Task<IEnumerable<Organization>> GetAllPublicOrganizationsAsync();
        Task<IEnumerable<Activity>> GetAllUserActivity(string organizationSlug, int userId);
        Task ValidateAllUserActivitiesAsync(string organizationSlug, int userId);
    }

    public class EFOrganizationRepositoryAdapter : IOrganizationRepository
    {
        private ThanksDBContext _dbContext;

        public EFOrganizationRepositoryAdapter(ThanksDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Organization>> GetAllPublicOrganizationsAsync()
        {
            return await _dbContext.Organizations.Where(o=>o.Visibility == OrganizationVisibility.Public).ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllUserActivity(string organizationSlug, int userId)
        {
            Organization organization = _dbContext.Organizations.First(o => o.Slug == organizationSlug);
            return _dbContext.Activities.Where(a => a.VolunteerId == userId && a.OrganizationId == organization.Id).Select(a => new Activity()
            {
                Id = a.Id,
                Name = a.Name,
                Status = a.Status,
                Date = a.Date,
                Duration = a.Duration
            });
        }

        public async Task ValidateAllUserActivitiesAsync(string organizationSlug, int userId)
        {
            Organization organization = _dbContext.Organizations.First(o => o.Slug == organizationSlug);
            await _dbContext.Activities.Where(a => a.VolunteerId == userId && a.OrganizationId == organization.Id).
                ExecuteUpdateAsync(a => a.SetProperty(p => p.Status, v => ActivityStatus.Approved));
        }
    }
}
