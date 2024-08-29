using WebApplication1.Models;


namespace WebApplication1.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
        Task<Organization?> GetOrganizationByIdAsync(int id);
        Task<Organization?> GetOrganizationWithDetailsByIdAsync(int id);
        Task AddOrganizationAsync(Organization organization);
        Task UpdateOrganizationAsync(Organization organization);
        Task DeleteOrganizationAsync(int id);
        Task<bool> OrganizationExistsAsync(int id);
    }
}
