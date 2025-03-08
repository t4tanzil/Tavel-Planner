using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Data;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                //Add OrderBy for A-Z sorting
                .Include(c => c.Country)  
                .OrderBy(c => c.Name)     
                .ToListAsync();  
            return View(cities);
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            return View();
        }

        // POST: Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityId,CountryId,Name,IsCapital")] City city)
        {
            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", city.CountryId);
                return View(city);
            }
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CityId,CountryId,Name,IsCapital")] City city)
        {
            if (id != city.CityId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", city.CountryId);
                return View(city);
            }
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Get city with related entities
                var city = await _context.Cities
                    .Include(c => c.Attractions)
                    .Include(c => c.Hotels)
                    .FirstOrDefaultAsync(c => c.CityId == id);

                if (city == null)
                {
                    return NotFound();
                }

                // First remove all hotels in this city
                if (city.Hotels != null && city.Hotels.Any())
                {
                    // For each hotel, check if there are bookings
                    foreach (var hotel in city.Hotels)
                    {
                        var bookings = await _context.Bookings
                            .Where(b => b.HotelId == hotel.HotelId)
                            .ToListAsync();

                        if (bookings.Any())
                        {
                            _context.Bookings.RemoveRange(bookings);
                        }
                    }

                    _context.Hotels.RemoveRange(city.Hotels);
                }

                // Then remove all attractions in this city
                if (city.Attractions != null && city.Attractions.Any())
                {
                    // For each attraction, check if there are bookings
                    foreach (var attraction in city.Attractions)
                    {
                        var bookings = await _context.Bookings
                            .Where(b => b.AttractionId == attraction.AttractionId)
                            .ToListAsync();

                        if (bookings.Any())
                        {
                            _context.Bookings.RemoveRange(bookings);
                        }

                        // Also check for hotels related to this attraction
                        var hotels = await _context.Hotels
                            .Where(h => h.AttractionId == attraction.AttractionId)
                            .ToListAsync();

                        if (hotels.Any())
                        {
                            _context.Hotels.RemoveRange(hotels);
                        }
                    }

                    _context.Attractions.RemoveRange(city.Attractions);
                }

                // Finally remove the city
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting city: {ex.Message}");
                ModelState.AddModelError("", "Unable to delete city. Try deleting related records first.");
                var city = await _context.Cities
                    .Include(c => c.Country)
                    .FirstOrDefaultAsync(m => m.CityId == id);
                return View(city);
            }
        }
    }
}
