using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly AppliactionDbContext _context;
        private readonly EmployeeService _employeeService;

        public TrainingsController(AppliactionDbContext context, EmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // GET: Trainings
        public async Task<IActionResult> Index()
        {
            var appliactionDbContext = _context.Trainings.Include(t => t.Organization);
            return View(await appliactionDbContext.ToListAsync());
        }

        // GET: Trainings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings
                .Include(t => t.Organization)
                .Include(t => t.TrainingEmployees)
                .ThenInclude(te => te.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // GET: Trainings/Create
        public IActionResult Create()
        {
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name");
            ViewData["Employees"] = new List<Employee>();
            return View();
        }

        // POST: Trainings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrainingDate,OrganizationId,PlaceOfTraining,PurposeOfTraining")] Training training, int[] employeeIds)
        {

            Console.WriteLine("Entered ");
            if (ModelState.IsValid)
            {

                Console.WriteLine("Valid State");
                _context.Add(training);
                await _context.SaveChangesAsync();

                var savedTraining = await _context.Trainings
                                          .Include(t => t.TrainingEmployees)
                                          .SingleOrDefaultAsync(t => t.Id == training.Id);
                if (savedTraining != null)
                {

                    Console.WriteLine("Not Saved");
                    Console.WriteLine(employeeIds.Length);
                    foreach (var employeeId in employeeIds)
                    {
                        var employee = await _context.Employees.FindAsync(employeeId);
                        if (employee != null)
                        {

                            Console.WriteLine("No Employee");
                            var trainingEmployee = new TrainingEmployee
                            {
                                TrainingId = training.Id,
                                EmployeeId = employeeId
                            };
                            _context.Add(trainingEmployee);

                            Console.WriteLine("trainingEmployee: ", trainingEmployee);
                        } else
                        {
                            Console.WriteLine("Employee is emplty");
                        }
                    }
                } else
                {

                    Console.WriteLine("Model State is not valid");
                    // Handle the case where the training was not found
                    ModelState.AddModelError("", "Training record not found.");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", training.OrganizationId);

            // Debugging information
            Console.WriteLine("Training ID: " + training.Id);
            Console.WriteLine("Selected Employee IDs: " + string.Join(", ", employeeIds));

            return View(training);
        }

        // GET: Trainings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var training = await _context.Trainings.FindAsync(id);

            var training = await _context.Trainings
                .Include(t => t.Organization)
                .Include(t => t.TrainingEmployees)
                    .ThenInclude(te => te.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (training == null)
            {
                return NotFound();
            }
            //ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", training.OrganizationId);
            ViewBag.OrganizationName = _context.Organizations
                .Where(o => o.Id == training.OrganizationId)
                .Select(o => o.Name)
                .FirstOrDefault();

            ViewData["SelectedEmployeeIds"] = training.TrainingEmployees.Select(te => te.EmployeeId).ToArray();
            return View(training);
        }

        // POST: Trainings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrainingDate,OrganizationId,PlaceOfTraining,PurposeOfTraining")] Training training, int[] employeeIds)
        {
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the training details
                    _context.Update(training);
                    await _context.SaveChangesAsync();

                    // Remove existing TrainingEmployee records
                    var existingTrainingEmployees = _context.TrainingEmployees
                        .Where(te => te.TrainingId == training.Id);
                    _context.TrainingEmployees.RemoveRange(existingTrainingEmployees);

                    // Add new TrainingEmployee records
                    if (employeeIds != null)
                    {
                        foreach (var employeeId in employeeIds)
                        {
                            var trainingEmployee = new TrainingEmployee
                            {
                                TrainingId = training.Id,
                                EmployeeId = employeeId
                            };
                            _context.Add(trainingEmployee);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", training.OrganizationId);
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

            var training = await _context.Trainings
                .Include(t => t.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            //var training = await _context.Trainings.FindAsync(id);
            var training = await _context.Trainings
                                 .Include(t => t.TrainingEmployees) // Include related TrainingEmployees
                                 .FirstOrDefaultAsync(t => t.Id == id);

            if (training != null)
            {
                _context.TrainingEmployees.RemoveRange(training.TrainingEmployees);
                _context.Trainings.Remove(training);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }

        // Get Employees after selecting Organization
        public IActionResult GetEmployeesFromOrganization(int organizationId) { 
            var employees = _context.Employees
                .Where(e => e.OrganizationId == organizationId)
                .ToList();

            return Json(employees);
        }
    }
}
