using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly AppliactionDbContext _context;

        public TrainingRepository(AppliactionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Training>> GetAllTrainingsAsync()
        {
            return await _context.Trainings
                .Include(t => t.Organization)
                .ToListAsync();
        }

        public async Task<Training?> GetTrainingByIdAsync(int id)
        {
            return await _context.Trainings
                .Include(t => t.Organization)
                .Include(t => t.TrainingEmployees)
                    .ThenInclude(te => te.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddTrainingAsync(Training training)
        {
            _context.Add(training);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTrainingAsync(Training training)
        {
            _context.Update(training);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTrainingAsync(int id)
        {
            var training = await _context.Trainings
                .Include(t => t.TrainingEmployees)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (training != null)
            {
                _context.TrainingEmployees.RemoveRange(training.TrainingEmployees);

                _context.Trainings.Remove(training);

                await _context.SaveChangesAsync();
            }
        }

        public bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }

        public async Task AddTrainingEmployeeAsync(TrainingEmployee trainingEmployee)
        {
            _context.Add(trainingEmployee);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllTrainingEmployeesAsync(int trainingId)
        {
            var trainingEmployees = _context.TrainingEmployees
                .Where(te => te.TrainingId == trainingId);

            _context.TrainingEmployees.RemoveRange(trainingEmployees);
            await _context.SaveChangesAsync();
        }
    }
}
