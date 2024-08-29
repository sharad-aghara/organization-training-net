using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppliactionDbContext _context;

        public EmployeeRepository(AppliactionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Organization)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Organization)
                .Include(e => e.TrainingEmployees)
                    .ThenInclude(te => te.Training)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            _context.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByOrganizationIdAsync(int organizationId)
        {
            return await _context.Employees
                .Where(e => e.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int organizationId, DateOnly trainingDate)
        {
            var engagedEmployeeIds = await _context.TrainingEmployees
                .Where(te => te.Training.TrainingDate == trainingDate)
                .Select(te => te.EmployeeId)
                .ToListAsync();

            return await _context.Employees
                .Where(e => e.OrganizationId == organizationId && !engagedEmployeeIds.Contains(e.Id))
                .ToListAsync();
        }
    }
}
