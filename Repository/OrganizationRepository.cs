using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly AppliactionDbContext _context;

        public OrganizationRepository(AppliactionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await _context.Organizations.ToListAsync();
        }

        public async Task<Organization?> GetOrganizationByIdAsync(int id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        public async Task<Organization?> GetOrganizationWithDetailsByIdAsync(int id)
        {
            return await _context.Organizations
                .Include(o => o.Training)
                .Include(o => o.Employees)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddOrganizationAsync(Organization organization)
        {
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            _context.Organizations.Update(organization);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            var organization = await GetOrganizationWithDetailsByIdAsync(id);
            if (organization != null)
            {
                _context.Employees.RemoveRange(organization.Employees);
                _context.Trainings.RemoveRange(organization.Training);
                _context.Organizations.Remove(organization);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> OrganizationExistsAsync(int id)
        {
            return await _context.Organizations.AnyAsync(e => e.Id == id);
        }
    }
}
