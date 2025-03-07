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
    public class AttractionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttractionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attractions
        public async Task<IActionResult> Index()
        {
            var attractions = await _context.Attractions
                .Include(a => a.Country)
                .Include(a => a.City)
                .ToListAsync();
            return View(attractions);
        }

        // GET: Attractions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attraction = await _context.Attractions
                .Include(a => a.Country)
                .Include(a => a.City)
                .FirstOrDefaultAsync(m => m.AttractionId == id);
            if (attraction == null)
            {
                return NotFound();
            }

            return View(attraction);
        }

        // GET: Attractions/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name");
            return View();
        }

        // POST: Attractions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttractionId,CountryId,CityId,Name,Type,Rating,BudgetLevel,Description")] Attraction attraction)
        {
            try
            {
                _context.Attractions.Add(attraction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", attraction.CountryId);
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", attraction.CityId);
                return View(attraction);
            }
        }

        // GET: Attractions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", attraction.CountryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", attraction.CityId);
            return View(attraction);
        }

        // POST: Attractions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttractionId,CountryId,CityId,Name,Type,Rating,BudgetLevel,Description")] Attraction attraction)
        {
            if (id != attraction.AttractionId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(attraction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", attraction.CountryId);
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", attraction.CityId);
                return View(attraction);
            }
        }

        // GET: Attractions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attraction = await _context.Attractions
                .Include(a => a.Country)
                .Include(a => a.City)
                .FirstOrDefaultAsync(m => m.AttractionId == id);
            if (attraction == null)
            {
                return NotFound();
            }

            return View(attraction);
        }

        // POST: Attractions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Find the attraction with related entities
                var attraction = await _context.Attractions
                    .Include(a => a.Hotels)
                    .Include(a => a.Bookings)
                    .FirstOrDefaultAsync(a => a.AttractionId == id);

                if (attraction == null)
                {
                    return NotFound();
                }

                // First remove all bookings related to this attraction
                if (attraction.Bookings != null && attraction.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(attraction.Bookings);
                }

                // Then remove all hotels related to this attraction
                if (attraction.Hotels != null && attraction.Hotels.Any())
                {
                    // For each hotel, check if there are bookings
                    foreach (var hotel in attraction.Hotels)
                    {
                        var bookings = await _context.Bookings
                            .Where(b => b.HotelId == hotel.HotelId)
                            .ToListAsync();

                        if (bookings.Any())
                        {
                            _context.Bookings.RemoveRange(bookings);
                        }
                    }

                    _context.Hotels.RemoveRange(attraction.Hotels);
                }

                // Finally remove the attraction
                _context.Attractions.Remove(attraction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting attraction: {ex.Message}");
                ModelState.AddModelError("", "Unable to delete attraction. Try deleting related records first.");
                var attraction = await _context.Attractions
                    .Include(a => a.Country)
                    .Include(a => a.City)
                    .FirstOrDefaultAsync(m => m.AttractionId == id);
                return View(attraction);
            }
        }
    }
}