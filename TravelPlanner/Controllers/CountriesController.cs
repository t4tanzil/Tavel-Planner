using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Data;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            var countries = await _context.Countries
                // Add OrderBy for A-Z sorting
                .OrderBy(c => c.Name)  
                .ToListAsync();
            return View(countries);
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryId,Name,Region,Currency,Language")] Country country)
        {
            try
            {
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return View(country);
            }
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,Name,Region,Currency,Language")] Country country)
        {
            if (id != country.CountryId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return View(country);
            }
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // First check if this country has related records
                var country = await _context.Countries
                    .Include(c => c.Cities)
                    .Include(c => c.Attractions)
                    .Include(c => c.Bookings)
                    .FirstOrDefaultAsync(c => c.CountryId == id);

                if (country == null)
                {
                    return NotFound();
                }

                // If there are related records, delete them first
                if (country.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(country.Bookings);
                }

                if (country.Attractions.Any())
                {
                    // For each attraction, delete related hotels
                    foreach (var attraction in country.Attractions)
                    {
                        var hotels = await _context.Hotels
                            .Where(h => h.AttractionId == attraction.AttractionId)
                            .ToListAsync();

                        if (hotels.Any())
                        {
                            _context.Hotels.RemoveRange(hotels);
                        }
                    }

                    _context.Attractions.RemoveRange(country.Attractions);
                }

                if (country.Cities.Any())
                {
                    _context.Cities.RemoveRange(country.Cities);
                }

                // Finally, delete the country
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting country: {ex.Message}");
                ModelState.AddModelError("", "Unable to delete country. Try deleting related records first.");
                var country = await _context.Countries.FindAsync(id);
                return View(country);
            }
        }
    }
}
