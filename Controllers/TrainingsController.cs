using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly AppliactionDbContext _context;
        private readonly ITrainingRepository _trainingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public TrainingsController(
            AppliactionDbContext context,
            ITrainingRepository trainingRepository,
            IEmployeeRepository employeeRepository,
            IOrganizationRepository organizationRepository)
        {
            _context = context;
            _trainingRepository = trainingRepository;
            _employeeRepository = employeeRepository;
            _organizationRepository = organizationRepository;
        }

        // GET: Trainings
        public async Task<IActionResult> Index()
        {
            var trainings = await _trainingRepository.GetAllTrainingsAsync();
            return View(trainings);
        }

        // GET: Trainings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _trainingRepository.GetTrainingByIdAsync(id.Value);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // GET: Trainings/Create
        public async Task<IActionResult> Create()
        {
            var organizations = await _organizationRepository.GetAllOrganizationsAsync();
            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name");
            ViewData["Employees"] = new List<Employee>();
            return View();
        }

        // POST: Trainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,TrainingDate,OrganizationId,PlaceOfTraining,PurposeOfTraining")] Training training,
            int[] employeeIds)
        {
            if (ModelState.IsValid)
            {
                await _trainingRepository.AddTrainingAsync(training);

                foreach (var employeeId in employeeIds)
                {
                    var trainingEmployee = new TrainingEmployee
                    {
                        TrainingId = training.Id,
                        EmployeeId = employeeId
                    };
                    //_context.Add(trainingEmployee);
                    await _trainingRepository.AddTrainingEmployeeAsync(trainingEmployee);
                }

                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            var organizations = await _organizationRepository.GetAllOrganizationsAsync();
            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name", training.OrganizationId);
            ViewData["SelectedEmployeeIds"] = employeeIds;

            return View(training);
        }

        // GET: Trainings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _trainingRepository.GetTrainingByIdAsync(id.Value);
            if (training == null)
            {
                return NotFound();
            }

            var organizations = await _organizationRepository.GetAllOrganizationsAsync();
            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name", training.OrganizationId);

            var employeeById = await _employeeRepository.GetEmployeesByOrganizationIdAsync(training.OrganizationId);
            ViewData["Employees"] = employeeById;

            ViewData["SelectedEmployeeIds"] = training.TrainingEmployees.Select(te => te.EmployeeId).ToArray();

            return View(training);
        }

        // POST: Trainings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,TrainingDate,OrganizationId,PlaceOfTraining,PurposeOfTraining")] Training training,
            int[] employeeIds)
        {
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _trainingRepository.UpdateTrainingAsync(training);

                    // Remove existing TrainingEmployee records
                    await _trainingRepository.RemoveAllTrainingEmployeesAsync(training.Id);

                    // Add new TrainingEmployee records
                    foreach (var employeeId in employeeIds)
                    {
                        var trainingEmployee = new TrainingEmployee
                        {
                            TrainingId = training.Id,
                            EmployeeId = employeeId
                        };
                        await _trainingRepository.AddTrainingEmployeeAsync(trainingEmployee);
                    }

                    return RedirectToAction(nameof(Details), new { id = training.Id });
                }
                catch
                {
                    if (!_trainingRepository.TrainingExists(training.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["OrganizationId"] = new SelectList(
                await _organizationRepository.GetAllOrganizationsAsync(), "Id", "Name", training.OrganizationId);
            ViewData["SelectedEmployeeIds"] = employeeIds;

            return View(training);
        }


        // GET: Trainings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _trainingRepository.GetTrainingByIdAsync(id.Value);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // POST: Trainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _trainingRepository.DeleteTrainingAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Get Employees after selecting Organization
        public async Task<IActionResult> GetEmployeesFromOrganization(int organizationId)
        {

            var employees = await _employeeRepository.GetEmployeesByOrganizationIdAsync(organizationId);
            var employeeList = employees.Select(e => new { e.Id, e.Name }).ToList();

            return Json(employeeList);
        }

        public async Task<IEnumerable<Employee>> GetAvailableEmployees(int organizationId, DateOnly trainingDate)
        {
            return await _employeeRepository.GetAvailableEmployeesAsync(organizationId, trainingDate);
        }

    }
}
