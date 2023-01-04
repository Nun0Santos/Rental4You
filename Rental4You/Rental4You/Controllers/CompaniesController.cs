using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;
using static Rental4You.Data.Initialization;

namespace Rental4You.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Companies
        [Authorize(Roles = "Admin, Manager, Employee")]    
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Manager") || User.IsInRole("Employee"))
            {
                var user = _context.Users.Find(_userManager.GetUserId(User));
                var companies = _context.Company.ToList();

                foreach (var c in companies)
                {
                    if (c.Id == user.CompanyId)
                    {
                        return View(await _context.Company.Where(d => d.Id == c.Id).ToListAsync());
                    }
                }

                return View("Error");
            }

              return View(await _context.Company.ToListAsync());
        }

        // GET: Companies/Details/5
        [Authorize(Roles = "Admin, Manager, Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Rating")] Company company)
        {
            ModelState.Remove(nameof(company.Employees));
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();

                var manager = new ApplicationUser
                {
                    UserName = "manager@" + company.Name + ".com",
                    Email = "manager@" + company.Name + ".com",
                    Name = "Manager",
                    Surname = company.Name,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    isActive = true,
                    CompanyId = company.Id,
                };
                var user = await _userManager.FindByEmailAsync(manager.Email);
                if (user == null)
                {
                    await _userManager.CreateAsync(manager, "1234Qaz.");
                    await _userManager.AddToRoleAsync(manager, Roles.Manager.ToString());
                }

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Rating")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(company.Employees));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Company == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Company'  is null.");
            }
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return _context.Company.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult CreateEmployees(int id)
        {
            var viewModel = new UserCreationViewModel();
            viewModel.CompanyId = id;

            var roles = _context.Roles;

            ViewData["Roles"] = new SelectList(roles.Where(c => c.Name == "Manager" || c.Name == "Employee"), "Name");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployees([Bind("CompanyId, UserName, Email, Name, Surname, Role")] UserCreationViewModel user)
        {
            var company = _context.Company.Find(user.CompanyId);
            if (company == null)
            {
                return View("Error");
            }

            if (ModelState.IsValid)
            {
                var NewUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    isActive = true,
                    CompanyId = user.CompanyId,
                };
                var u = await _userManager.FindByEmailAsync(NewUser.Email);
                if (u == null)
                {
                    await _userManager.CreateAsync(NewUser, "1234Qaz.");
                    if (user.Role == "Employee")
                    {
                        await _userManager.AddToRoleAsync(NewUser, Roles.Employee.ToString());
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(NewUser, Roles.Manager.ToString());
                    }

                    company.Employees.Add(NewUser);
                }
                else
                {
                    return View("Error");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Employees(int id)
        {
            var employees = _context.Users;

            return View(employees.Where(c => c.Id != _userManager.GetUserId(User) && c.CompanyId == id));
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ActivateEmployees(string id)
        {
            var employee = _context.Users.Find(id);
            if (employee.isActive)
            {
                employee.isActive = false;
                _context.Update(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Employees), new {id = employee.CompanyId});
            }

            employee.isActive = true;
            _context.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Employees), new { id = employee.CompanyId });
        }
    }
}
