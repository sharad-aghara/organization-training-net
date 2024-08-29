using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        bool EmployeeExists(int id);

        Task<IEnumerable<Employee>> GetEmployeesByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int organizationId, DateOnly trainingDate);
    }
}
