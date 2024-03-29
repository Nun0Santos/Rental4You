﻿
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

namespace Rental4You.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Cars

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
                        return View(await _context.cars.Include(c => c.Company)
                            .Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id).ToListAsync());
                       
                    }
                }
            }


            return View(await _context.cars.Include(c => c.Company).Where(c => c.isActive == true && c.isReserved == false 
            && c.Company.isActive == true && c.Category.isActive == true).ToListAsync());
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
        [Authorize(Roles = "Employee, Manager")]
        public IActionResult Create()
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive), "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Manager")]
        public async Task<IActionResult> Create([Bind("Id,Maker,Model,CategoryId,Transmission,Seats,Year,LicensePlate,Location,Km,state,price,fuel, CompanyId")] Car car, [FromForm] List<IFormFile> files)
        {
            ViewData["Companies"] = new SelectList(_context.Company.ToList(), "Id", "Name");
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive), "Id", "Name");
            ModelState.Remove(nameof(car.Company));
            ModelState.Remove(nameof(car.Category));

            car.isActive = true;
            car.isReserved = false;
            car.CompanyId = (int)_context.Users.Find(_userManager.GetUserId(User)).CompanyId;

            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();

                if (!Directory.Exists("wwwroot\\uploads"))
                {
                    Directory.CreateDirectory("wwwroot\\uploads");
                }
                if (!Directory.Exists("wwwroot\\uploads\\cars"))
                {
                    Directory.CreateDirectory("wwwroot\\uploads\\cars");
                }
                Directory.CreateDirectory("wwwroot\\uploads\\cars\\" + car.Id);

                foreach (IFormFile file in files)
                {
                    var fs = System.IO.File.Create("wwwroot\\uploads\\cars\\" + car.Id + "\\" + file.FileName);
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.WriteTo(fs);
                    }
                    fs.Close();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Employee, Manager")]
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

            ViewData["Companies"] = new SelectList(_context.Company.ToList(), "Id", "Name");
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true).ToList(), "Id", "Name");

            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Maker,Model,CategoryId,Transmission,Seats,Year,LicensePlate,Location,Km,state, price, fuel, CompanyId, isActive, isReserved")] Car car, [FromForm]List<IFormFile> files)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true).ToList(), "Id", "Name");
            ViewData["Companies"] = new SelectList(_context.Company.ToList(), "Id", "Name");

            ModelState.Remove(nameof(car.Company));
            ModelState.Remove(nameof(car.Category));

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
                    if (!Directory.Exists("wwwroot\\uploads"))
                    {
                        Directory.CreateDirectory("wwwroot\\uploads");
                    }
                    if (!Directory.Exists("wwwroot\\uploads\\cars"))
                    {
                        Directory.CreateDirectory("wwwroot\\uploads\\cars");
                    }
                    if (!Directory.Exists("wwwroot\\uploads\\cars\\" + car.Id))
                    {
                        Directory.CreateDirectory("wwwroot\\uploads\\cars\\" + car.Id);
                    }

                    foreach (IFormFile file in files)
                    {
                        var fs = System.IO.File.Create("wwwroot\\uploads\\cars\\" + car.Id + "\\" + file.FileName);
                        using (var stream = new MemoryStream())
                        {
                            await file.CopyToAsync(stream);
                            stream.WriteTo(fs);
                        }
                        fs.Close();
                    }
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
        [Authorize(Roles = "Employee, Manager")]
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
        [Authorize(Roles = "Employee, Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            if (_context.cars == null)
            {
                return Problem("Entity set 'ApplicationDbContext.cars'  is null.");
            }
            var car = await _context.cars.FindAsync(id);

            var reservations =  _context.reservations.Where(c => c.CarId == car.Id && c.isDelivered == false);
            if (reservations.Count() > 0)
            {
                return View("Error");
            }

            if (car != null)
            {
                if (Directory.Exists("wwwroot\\uploads\\cars\\" + id))
                {
                    Directory.Delete("wwwroot\\uploads\\cars\\" + id, true);
                }
                _context.cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Employee, Manager")]
        public async Task<IActionResult> deleteImage(int id, string imgPath)
        {
            System.IO.File.Delete("wwwroot\\" + imgPath);
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        private bool CarExists(int id)
        {
            return _context.cars.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Search([Bind("PickupLocation,VehicleType,PickupDate,ReturnDate")] SearchCarViewModel search)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                return View("../Home");
            }
            SearchCarViewModel searchCar = new SearchCarViewModel();

            if (string.IsNullOrWhiteSpace(search.VehicleType) && string.IsNullOrWhiteSpace(search.PickupLocation))
            {
                searchCar.ListOfCars = await _context.cars.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.isActive == true && c.isReserved == false && c.Company.isActive == true && c.Category.isActive == true).ToListAsync();
                return View(searchCar);
            }

            if (string.IsNullOrWhiteSpace(search.PickupLocation))
            {
                searchCar.ListOfCars = await _context.cars.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.isActive == true && c.isReserved == false && c.Company.isActive == true && c.Category.Id == int.Parse(search.VehicleType)).ToListAsync();

                searchCar.TextToSearch = search.VehicleType;
                searchCar.NumberOfResults = searchCar.ListOfCars.Count();
                return View(searchCar);

            }

            if (string.IsNullOrWhiteSpace(search.VehicleType))
            {
                searchCar.ListOfCars = await _context.cars.Include(c => c.Company).Include(c => c.Category)
                   .Where(c => c.Location.Contains(search.PickupLocation)
                        && c.isActive == true && c.isReserved == false && c.Company.isActive == true).ToListAsync();

                searchCar.TextToSearch = search.PickupLocation;
                searchCar.NumberOfResults = searchCar.ListOfCars.Count();
                return View(searchCar);


            }
            
            searchCar.ListOfCars = await _context.cars.Include(c => c.Company).Include(c => c.Category)
                .Where(c => c.Category.Id == int.Parse(search.VehicleType) &&  c.Location.Contains(search.PickupLocation)
                         && c.isActive == true && c.isReserved == false && c.Company.isActive == true && c.Category.isActive == true).ToListAsync();

            searchCar.TextToSearch = search.PickupLocation + " Type: " + search.VehicleType;
            searchCar.NumberOfResults = searchCar.ListOfCars.Count();

            return View(searchCar);
        }
        public ActionResult GetProducts(string sort)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");

            var carlist = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        if (sort == "ascending")
                        {
                            return View("Index", _context.cars.OrderBy(p => p.price).Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.Category.isActive == true));
                        }

                        return View("Index", _context.cars.OrderByDescending(p => p.price).Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.Category.isActive == true));
                    }
                }
            }

            if (sort == "ascending")
            {
                var sortedProductsGrowing = carlist.OrderBy(p => p.price);
                return View("Index", sortedProductsGrowing.Include(c => c.Company).Include(c => c.Category)
                                    .Where(c => c.isActive == true && c.isReserved == false && c.Company.isActive == true && c.Category.isActive == true));
            }
            else if (sort == "descending")
            {
                var sortedProductsDescending = carlist.OrderByDescending(p => p.price);
                return View("Index", sortedProductsDescending.Include(c => c.Company).Include(c => c.Category)
                                    .Where(c => c.isActive == true && c.isReserved == false && c.Company.isActive == true && c.Category.isActive == true));

            }
            else
            {
                return RedirectToAction(nameof(Index));

            }
        }

        public ActionResult OrderByRating(string requirement)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var carlist = _context.cars.Include(c => c.Company);

            if (requirement == "ascending")
            {
                var list = carlist.OrderBy(c => c.Company.Rating).Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.isActive == true 
                && c.isReserved == false && c.Company.isActive == true && c.Category.isActive == true);
                return View("Index", list);
            }

            if (requirement == "descending")
            {
                var list = carlist.OrderByDescending(c => c.Company.Rating).Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.isActive == true && c.isReserved == false
                && c.Company.isActive == true && c.Category.isActive == true);
                return View("Index", list);
            }

            return View("Index", carlist.Include(c => c.Company).Include(c => c.Category).Where(c => c.isActive == true && c.isReserved == false && c.Category.isActive == true));
        }

        public ActionResult GetCategories(int requirement)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var carlist = _context.cars;

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
                            return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.Category.Id == requirement && c.Category.isActive == true));
                        }

                        return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.Category.isActive == true));
                    }
                }
            }

            if (requirement == 0)
            {
                return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.Category.Id == requirement && c.Category.isActive == true));
            }
            return View("Index", carlist.Include(c => c.Company).Include(c => c.Category)
                .Where(c => c.isActive == true && c.isReserved == false
            && c.Company.isActive == true && c.Category.Id == requirement));
        }


        public ActionResult GetFuel(string requirement)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var carlist = _context.cars;

            if (User.IsInRole("Employee") || User.IsInRole("Manager"))
            {
                var userList = _context.Users.Find(_userManager.GetUserId(User));
                var companyList = _context.Company;

                foreach (var u in companyList)
                {
                    if (u.Id == userList.CompanyId)
                    {
                        if (requirement == "oil")
                        {
                            return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.fuel == "oil" && c.Category.isActive == true));
                        }
                        if (requirement == "gas")
                        {
                            return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.fuel == "gas" && c.Category.isActive == true));
                        }
                        if (requirement == "hybrid")
                        {
                            return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.fuel == "hybrid" && c.Category.isActive == true));
                        }
                        if (requirement == "electric")
                        {
                            return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.fuel == "electric" && c.Category.isActive == true));
                        }

                        return View("Index", _context.cars.Include(c => c.Company).Include(c => c.Category)
                            .Where(c => c.CompanyId == u.Id && c.Category.isActive == true));
                    }
                }
            }

            if (requirement == "oil")
            {
                return View("Index", carlist.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.fuel == "oil" && c.isActive == true && c.isReserved == false
                && c.Company.isActive == true && c.Category.isActive == true));
            }
            if (requirement == "gas")
            {
                return View("Index", carlist.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.fuel == "gas" && c.isActive == true && c.isReserved == false
                && c.Company.isActive == true && c.Category.isActive == true));
            }
            if (requirement == "electric")
            {
                return View("Index", carlist.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.fuel == "electric" && c.isActive == true && c.isReserved == false
                && c.Company.isActive == true && c.Category.isActive == true));
            }
            if (requirement == "hybrid")
            {
                return View("Index", carlist.Include(c => c.Company).Include(c => c.Category)
                    .Where(c => c.fuel == "hybrid" && c.isActive == true && c.isReserved == false
                && c.Company.isActive == true && c.Category.isActive == true));
            }
            return View("Index", carlist.Include(c => c.Company)
                .Where(c => c.isActive == true && c.isReserved == false && c.Category.isActive == true));
        }

        [Authorize(Roles = "Employee, Manager")]

        public ActionResult getState(string requirement)
        {
            ViewData["categories"] = new SelectList(_context.categories.Where(c => c.isActive == true), "Id", "Name");
            var carlist = _context.cars.Include(c => c.Company);
            var userList = _context.Users.Find(_userManager.GetUserId(User));

            var companyList = _context.Company;

            foreach (var u in companyList)
            {
                if (u.Id == userList.CompanyId)
                {
                    return View("Index", carlist
                        .Include(c => c.Company)
                        .Include(c => c.Category)
                        .Where(c => c.CompanyId == u.Id && c.state == requirement && c.Category.isActive == true));
                }
            }
            return View("Index", carlist
                       .Include(c => c.Company)
                       .Include(c => c.Category)
                       .Where(c => c.state == requirement && c.Category.isActive == true));
        }

    }
}
  