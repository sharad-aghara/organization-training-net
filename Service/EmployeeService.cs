using System.Data.Entity;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class EmployeeService
    {
        private readonly AppliactionDbContext _context;

        public EmployeeService(AppliactionDbContext context)
        {
            _context = context;
        }


        public async Task<List<Employee>> GetEmployeesByOrganization(int  organizationId)
        {
            return await _context.Employees
                .Where(e => e.OrganizationId == organizationId)
                .ToListAsync();
        }
    }
}
