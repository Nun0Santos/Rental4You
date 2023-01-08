using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.ViewModels;

namespace Rental4You.Models
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        [Authorize(Roles = "Admin, Employee, Manager")]
        public async Task<IActionResult> Index()
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        return View(await _context.reservations.Include(c => c.Car)
                            .Include(c=>c.Client)
                            .Include(c=>c.Car.Company)
                            .Where(c =>c.Car.CompanyId == u.Id).ToListAsync());
                    }
                }
            }

            var applicationDbContext = _context.reservations.Include(r => r.Car)
               .Include(r => r.Client)
               .Include(r => r.Car.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                .Include(r => r.Car)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }


        public IActionResult Booking(int id)
        {
            var carList = _context.cars.ToList();

            var viewModel = new ReservationViewModel();

            viewModel.carID = id;

            foreach (var car in carList)
            {
                if (car.Id == id)
                {
                    viewModel.carMaker = car.Maker;
                    viewModel.carModel = car.Model;
                }

            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Calculate([Bind("start, end, carID")] ReservationViewModel request)
        {
            ModelState.Remove(nameof(request.carModel));
            ModelState.Remove(nameof(request.carMaker));

            if (request.start > request.end)
            {
                ModelState.AddModelError("Start Date", "Start date cannot be after the end date");
            }
            if (ModelState.IsValid)
            {
                var id = request.carID;
                var carList = _context.cars;
                var car = carList.Find(request.carID);
                var nDays = (request.end - request.start).TotalDays;
                var nPrice = nDays * car.price;

                Reservation reservation = new Reservation();
                reservation.Start = request.start;
                reservation.End = request.end;
                reservation.Price = (decimal)nPrice;
                reservation.CarId = request.carID;
                reservation.Car = car;

                return View("ConfirmBooking", reservation);

            }

            return View("Booking", request);
        }


        // GET: Reservations/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id");
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([Bind("Id,Start,End,CarId,Price,Confirmed")] Reservation reservation)
        {
            ModelState.Remove(nameof(reservation.Car));
            
            reservation.ClientId = _userManager.GetUserId(User);

            ModelState.Remove(nameof(reservation.Client));
            ModelState.Remove(nameof(reservation.ClientId));

            reservation.isDelivered = false;

            var car = _context.cars.Find(reservation.CarId);
            car.isReserved = true;

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction(nameof(MyReservations));
                }
                return RedirectToAction(nameof(MyReservations));
            }
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", reservation.CarId);
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", reservation.ClientId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", reservation.CarId);
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", reservation.ClientId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End,CarId,ClientId")] Reservation reservation)
        {
            ModelState.Remove(nameof(reservation.Car));
            ModelState.Remove(nameof(reservation.Client));
            ModelState.Remove(nameof(reservation.Confirmed));
            ModelState.Remove(nameof(reservation.isDelivered));


            var car = _context.cars.Find(reservation.CarId);
            var days = (reservation.End - reservation.Start).TotalDays;
            reservation.Price = (decimal)(days * car.price);

            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyReservations));
            }
            ViewData["CarId"] = new SelectList(_context.cars, "Id", "Id", reservation.CarId);
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", reservation.ClientId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin, Employee, Manager, Client")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                .Include(r => r.Car)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee, Manager, Client")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            if (_context.reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.reservations'  is null.");
            }
            
            var reservation = await _context.reservations.FindAsync(id);
            if (reservation.Confirmed)
            {
                return Problem("This reservation has already been confirmed");
            }

            var car = _context.cars.Find(reservation.CarId);
            car.isReserved = false;
            
            if (reservation != null)
            {
                _context.reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            if (User.IsInRole("Client"))
            {
                return RedirectToAction(nameof(MyReservations));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return _context.reservations.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, Employee, Manager")]
        public async Task<IActionResult> confirmReservation(int id)
        {
            var reservation = await _context.reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Confirmed = true;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MyReservations()
        {
            var applicationDbContext = _context.reservations.Include(r => r.Car)
                .Include(r => r.Client)
                .Include(r => r.Car.Company)
                .Where(c => c.Client.Id == _userManager.GetUserId(User));

            return View(await applicationDbContext.ToListAsync());
        }
       

        [Authorize(Roles = "Admin,Employee,Manager")]
        public ActionResult GetDate(DateTime pickUpdate, DateTime returnDate)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var reservationsList = _context.reservations;
            var carList = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {

                        return View("Index", reservationsList
                                   .Include(c => c.Car)
                                   .Include(c => c.Client)
                                   .Include(c => c.Car.Company)
                                   .Where(c => c.Car.CompanyId == u.Id && c.Start.Date >= pickUpdate.Date && c.End.Date <= returnDate.Date));
                    }
                }
            }
             return View("Index", reservationsList
                                      .Include(c => c.Car)
                                      .Include(c => c.Client)
                                      .Include(c => c.Car.Company)
                                      .Where(c => c.Start.Date >= pickUpdate.Date && c.End.Date <= returnDate.Date));
            }

        [Authorize(Roles = "Admin,Employee,Manager")]
        public ActionResult GetCategories(int requirement)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var reservationsList = _context.reservations;
            var carList = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        if (requirement == 0)
                        {
                            return View("Index", reservationsList
                            .Include(c => c.Car).Include(c => c.Client)
                            .Include(c => c.Car.Company)
                            .Where(c => c.Car.CompanyId == u.Id));
                        }

                        return View("Index", reservationsList
                                .Include(c => c.Car).Include(c => c.Client)
                                .Include(c => c.Car.Company)
                                .Include(c => c.Car.Category)
                                .Where(c => c.Car.Category.Id == requirement && c.Car.CompanyId == u.Id));
                    }
                }
            }

            if (requirement == 0)
            {
                return View("Index", reservationsList
                .Include(c => c.Car).Include(c => c.Client)
                .Include(c => c.Car.Company));
            }

            return View("Index", reservationsList
                              .Include(c => c.Car).Include(c => c.Client)
                              .Include(c => c.Car.Company)
                              .Include(c => c.Car.Category)
                              .Where(c => c.Car.Category.Id == requirement));
        }

        [Authorize(Roles = "Admin,Employee,Manager")]
        public ActionResult GetMaker(string maker)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var reservationsList = _context.reservations;
            var carList = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        if (maker == "All makers")
                        {
                            return View("Index", reservationsList
                                .Include(c => c.Car)
                                .Include(c => c.Client)
                                .Include(c => c.Car.Company)
                                .Where(c => c.Car.CompanyId == u.Id));
                        }
                        return View("Index", reservationsList
                                .Include(c => c.Car)
                                .Include(c => c.Client)
                                .Include(c => c.Car.Company)
                                .Where(c => c.Car.CompanyId == u.Id && c.Car.Maker == maker));
                    }
                }
            }

            if (maker == "All makers")
            {
                return View("Index", reservationsList
                    .Include(c => c.Car)
                    .Include(c => c.Client)
                    .Include(c => c.Car.Company));
            }
            return View("Index", reservationsList
                   .Include(c => c.Car)
                   .Include(c => c.Client)
                   .Include(c => c.Car.Company)
                   .Where(c=> c.Car.Maker == maker));

        }

        [Authorize(Roles = "Admin,Employee,Manager")]
        public ActionResult GetClients(string client)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var reservationsList = _context.reservations;
            var carList = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        if (client == "All clients")
                        {
                            return View("Index", reservationsList
                                       .Include(c => c.Car)
                                       .Include(c => c.Client)
                                       .Include(c => c.Car.Company)
                                       .Where(c => c.Car.CompanyId == u.Id));
                        }
                        return View("Index", reservationsList
                               .Include(c => c.Car)
                               .Include(c => c.Client)
                               .Include(c => c.Car.Company)
                               .Where(c => c.Car.CompanyId == u.Id && c.Client.UserName == client));
                    }
                }
            }

            if (client == "All clients")
            {
                return View("Index", reservationsList
                           .Include(c => c.Car)
                           .Include(c => c.Client)
                           .Include(c => c.Car.Company));
            }

            return View("Index", reservationsList
                               .Include(c => c.Car)
                               .Include(c => c.Client)
                               .Include(c => c.Car.Company)
                               .Where(c => c.Client.UserName == client));

        }

    }
}
