using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.reservations.Include(r => r.car).Include(r => r.delivery);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Booking(int id)
        {
            var carList = _context.cars.ToList();

            var viewModel = new ReservationViewModel();

            viewModel.carID = id;
            viewModel.start = DateTime.Now.Date;
            viewModel.end = DateTime.Now.AddDays(1).Date;

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
                reservation.price = (float)nPrice;
                reservation.carId = request.carID;
                reservation.car = car;

                return View("ConfirmBooking", reservation);

            }

            return View("Booking", request);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                .Include(r => r.car)
                .Include(r => r.delivery)
             
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {    
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End,carId,deliveryId,returnalId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["carId"] = new SelectList(_context.cars, "Id", "Id", reservation.carId);
            ViewData["deliveryId"] = new SelectList(_context.deliveries, "Id", "Id", reservation.deliveryId);
        
            return View(reservation);
        }

        // GET: Reservations/Edit/5
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
            ViewData["carId"] = new SelectList(_context.cars, "Id", "Id", reservation.carId);
            ViewData["deliveryId"] = new SelectList(_context.deliveries, "Id", "Id", reservation.deliveryId);
      
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End,carId,deliveryId,returnalId")] Reservation reservation)
        {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["carId"] = new SelectList(_context.cars, "Id", "Id", reservation.carId);
            ViewData["deliveryId"] = new SelectList(_context.deliveries, "Id", "Id", reservation.deliveryId);
         
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                .Include(r => r.car)
                .Include(r => r.delivery)
            
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reservations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.reservations'  is null.");
            }
            var reservation = await _context.reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return _context.reservations.Any(e => e.Id == id);
        }
    }
}
