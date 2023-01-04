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
using static Rental4You.Data.Initialization;

namespace Rental4You.Models
{
    public class DeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Deliveries
        [Authorize(Roles = "Admin, Employee, Manager")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Employees.Contains(userList))
                    {
                        return View(await _context.deliveries
                            .Include(c => c.Car)
                            .Include(c => c.Car.Company)
                            .Include(d => d.Employee)
                            .Include(d => d.Reservation)
                            .Include(c => c.Reservation.Client)
                            .Where(c => c.Car.CompanyId == u.Id && c.isReceived == true).ToListAsync());
                    }
                }
            }

            var applicationDbContext = _context.deliveries.Include(d => d.Car)
                .Include(c => c.Car.Company)
                .Include(d => d.Employee)
                .Include(d => d.Reservation)
                .Include(c => c.Reservation.Client)
                .Where(c => c.isReceived == true);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee, Manager")]
        public async Task<IActionResult> DeliveryRequests(int id)
        {
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Employees.Contains(userList))
                    {
                        return View(await _context.deliveries
                            .Include(c => c.Car)
                            .Include(c => c.Car.Company)
                            .Include(d => d.Employee)
                            .Include(d => d.Reservation)
                            .Include(c => c.Reservation.Client)
                            .Where(c => c.Car.CompanyId == u.Id && c.isReceived == false).ToListAsync());
                    }
                }
            }
            var applicationDbContext = _context.deliveries.Include(d => d.Car)
                .Include(c => c.Car.Company)
                .Include(d => d.Employee)
                .Include(d => d.Reservation)
                .Include(c => c.Reservation.Client)
                .Where(c => c.isReceived == false);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries
                .Include(d => d.Car)
                .Include(d => d.Employee)
                .Include(d => d.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id");
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ReservationId"] = new SelectList(_context.reservations, "Id", "Id");
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Km,Damage,Observations,CarId,EmployeeId,ReservationId")] Delivery delivery, [FromForm] List<IFormFile> files)
        {
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id");
            ViewData["ReservationId"] = new SelectList(_context.reservations, "Id", "Id");

            ModelState.Remove(nameof(delivery.EmployeeId));
            ModelState.Remove(nameof(delivery.Car));
            ModelState.Remove(nameof(delivery.Employee));
            ModelState.Remove(nameof(delivery.Reservation));

            delivery.isReceived = false;
            delivery.DateOfDelivery = DateTime.Now;

            var reservation = _context.reservations.Find(delivery.ReservationId);
            delivery.CarId = reservation.CarId;

            if (ModelState.IsValid)
            {
                _context.Add(delivery);
                await _context.SaveChangesAsync();

                if (files != null && files.Count() != 0)
                {
                    if (!Directory.Exists("wwwroot\\uploads"))
                    {
                        Directory.CreateDirectory("wwwroot\\uploads");
                    }
                    if (!Directory.Exists("wwwroot\\uploads\\deliveries"))
                    {
                        Directory.CreateDirectory("wwwroot\\uploads\\deliveries");
                    }
                    Directory.CreateDirectory("wwwroot\\uploads\\deliveries\\" + delivery.Id);

                    foreach (IFormFile file in files)
                    {
                        var fs = System.IO.File.Create("wwwroot\\uploads\\deliveries\\" + delivery.Id + "\\" + file.FileName);
                        using (var stream = new MemoryStream())
                        {
                            await file.CopyToAsync(stream);
                            stream.WriteTo(fs);
                        }
                        fs.Close();
                    }
                }

                var reservations = _context.reservations.Include(r => r.Car)
                                            .Include(r => r.Client)
                                            .Include(r => r.Car.Company)
                                            .Where(c => c.Client.Id == _userManager.GetUserId(User));
                return View("../Reservations/MyReservations", reservations.ToList());
            }
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", delivery.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", delivery.EmployeeId);
            ViewData["ReservationId"] = new SelectList(_context.reservations, "Id", "Id", delivery.ReservationId);
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", delivery.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", delivery.EmployeeId);
            ViewData["ReservationId"] = new SelectList(_context.reservations, "Id", "Id", delivery.ReservationId);
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Km,Damage,Observations,CarId,DateOfDelivery,ReservationId")] Delivery delivery, [FromForm] List<IFormFile> files)
        {
            ModelState.Remove(nameof(delivery.Car));
            ModelState.Remove(nameof(delivery.Employee));
            ModelState.Remove(nameof(delivery.Reservation));

            if (id != delivery.Id || _context.deliveries == null)
            {
                return NotFound();
            }

            delivery.EmployeeId = _userManager.GetUserId(User);

            var reservation = await _context.reservations.FindAsync(delivery.ReservationId);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.isDelivered = true;
            delivery.isReceived = true;

            var car = _context.cars.Find(delivery.CarId);
            if (car == null)
            {
                return NotFound();
            }
            car.Km += delivery.Km;
            car.isReserved = false;

            if (delivery.Damage = true)
            {
                car.state = "Damaged";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                    if (delivery.Damage == true)
                    {
                        if (!Directory.Exists("wwwroot\\uploads"))
                        {
                            Directory.CreateDirectory("wwwroot\\uploads");
                        }
                        if (!Directory.Exists("wwwroot\\uploads\\deliveries"))
                        {
                            Directory.CreateDirectory("wwwroot\\uploads\\deliveries");
                        }
                        Directory.CreateDirectory("wwwroot\\uploads\\deliveries\\" + delivery.Id);

                        foreach (IFormFile file in files)
                        {
                            var fs = System.IO.File.Create("wwwroot\\uploads\\deliveries\\" + delivery.Id + "\\" + file.FileName);
                            using (var stream = new MemoryStream())
                            {
                                await file.CopyToAsync(stream);
                                stream.WriteTo(fs);
                            }
                            fs.Close();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
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
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", delivery.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", delivery.EmployeeId);
            ViewData["ReservationId"] = new SelectList(_context.reservations, "Id", "Id", delivery.ReservationId);
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries
                .Include(d => d.Car)
                .Include(d => d.Employee)
                .Include(d => d.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.deliveries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.deliveries'  is null.");
            }
            var delivery = await _context.deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.deliveries.Remove(delivery);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeliveryRequests));
        }

        private bool DeliveryExists(int id)
        {
          return _context.deliveries.Any(e => e.Id == id);
        }
    }
}
