
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rental4You.Data;
using Rental4You.Models;
using Rental4You.ViewModels;

namespace Rental4You.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            ViewData["ListTypes"] = new SelectList(_context.cars.ToList(), "Id","Name");


            return View(await _context.cars.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.cars == null)
            {
                return NotFound();
            }

            var car = await _context.cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Maker,Model,Type,Transmission,Seats,Year,LicensePlate,Location,Km,state")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.cars == null)
            {
                return NotFound();
            }

            var car = await _context.cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Maker,Model,Type,Transmission,Seats,Year,LicensePlate,Location,Km,state")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.cars == null)
            {
                return NotFound();
            }

            var car = await _context.cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.cars == null)
            {
                return Problem("Entity set 'ApplicationDbContext.cars'  is null.");
            }
            var car = await _context.cars.FindAsync(id);
            if (car != null)
            {
                _context.cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.cars.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Search([Bind("PickupLocation,VehicleType,PickupDate,ReturnDate")] SearchCarViewModel search)
        {
            SearchCarViewModel searchCar = new SearchCarViewModel();

            if (string.IsNullOrWhiteSpace(search.VehicleType) && string.IsNullOrWhiteSpace(search.PickupLocation))
            {
                searchCar.ListOfCars = await _context.cars.ToListAsync();
                return View(searchCar);
            }

            if (string.IsNullOrWhiteSpace(search.PickupLocation))
            {
                searchCar.ListOfCars = await _context.cars.
                    Where(c => c.Type.Contains(search.VehicleType)
                         ).ToListAsync();

                searchCar.TextToSearch = search.VehicleType;
                searchCar.NumberOfResults = searchCar.ListOfCars.Count();
                return View(searchCar);

            }

            if (string.IsNullOrWhiteSpace(search.VehicleType))
            {
                searchCar.ListOfCars = await _context.cars.
                   Where(c => c.Location.Contains(search.PickupLocation)
                        ).ToListAsync();

                searchCar.TextToSearch = search.PickupLocation;
                searchCar.NumberOfResults = searchCar.ListOfCars.Count();
                return View(searchCar);


            }
            
            searchCar.ListOfCars = await _context.cars.
                    Where(c => c.Type.Contains(search.VehicleType) &&  c.Location.Contains(search.PickupLocation)
                         ).ToListAsync();

            searchCar.TextToSearch = search.PickupLocation + " Type: " + search.VehicleType;
            searchCar.NumberOfResults = searchCar.ListOfCars.Count();

            return View(searchCar);
               
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Search([Bind("PickupLocation,VehicleType,PickupDate,ReturnDate")] SearchCarViewModel searchCar )
        //{

        //    if (string.IsNullOrEmpty(searchCar.TextToSearch))
        //    {
        //        searchCar.ListOfCars =
        //            await _context.cars.Include("marca").ToListAsync();

        //        searchCar.NumberOfResults = searchCar.ListOfCars.Count();
        //    }
        //    else
        //    {
        //        searchCar.ListOfCars =
        //            await _context.cars.Include(c => c.Maker).Where(
        //                c => c.Maker.Contains(searchCar.TextToSearch)
        //                ).ToListAsync();

        //        searchCar.NumberOfResults = searchCar.ListOfCars.Count();

        //    }

        //    return View(searchCar);
        //}
        public ActionResult GetProducts(string sort)
        {
            //List<Car> products = GetProducts();
            //List<Car> cars = new List<Car>();
            var carlist = _context.cars;

            if (sort == "ascending")
            {
                // Ordena a lista de produtos por preço crescente
               // SearchCarViewModel sortedProductsView = new SearchCarViewModel();
                var sortedProductsGrowing = carlist.OrderBy(p => p.price);
                return View("Index", sortedProductsGrowing);
            }
            else if (sort == "descending")
            {
                // Ordena a lista de produtos por preço decrescente
                //SearchCarViewModel sortedProductsView = new SearchCarViewModel();
                var sortedProductsDescending = carlist.OrderByDescending(p => p.price);
                return View("Index", sortedProductsDescending);
                //return RedirectToAction(nameof(Details), new { id = id});

            }
            else
            {
                // Não ordena a lista de produtos
                return RedirectToAction(nameof(Index));

            }
        }
        public ActionResult GetCategories(string sort)
        {
            return View(Index);
        }
    }

}
  