using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<Training>> GetAllTrainingsAsync();
        Task<Training?> GetTrainingByIdAsync(int id);
        Task AddTrainingAsync(Training training);
        Task UpdateTrainingAsync(Training training);
        Task DeleteTrainingAsync(int id);
        bool TrainingExists(int id);

        Task AddTrainingEmployeeAsync(TrainingEmployee trainingEmployee);
        Task RemoveAllTrainingEmployeesAsync(int trainingId);
    }
}
