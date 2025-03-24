using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Data;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                //Add OrderBy for A-Z sorting
                .OrderBy(c => c.Name) 
                .Include(h => h.Attraction)
                .Include(h => h.City)
                .ToListAsync();
            return View(hotels);
        }

        // GET: Hotels/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Attraction)
                .Include(h => h.City)
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name");
            return View();
        }

        // POST: Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,AttractionId,CityId,Name,Stars,Price,Address,Contact")] Hotel hotel)
        {
            try
            {
                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", hotel.AttractionId);
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", hotel.CityId);
                return View(hotel);
            }
        }

        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", hotel.AttractionId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", hotel.CityId);
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,AttractionId,CityId,Name,Stars,Price,Address,Contact")] Hotel hotel)
        {
            if (id != hotel.HotelId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", hotel.AttractionId);
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", hotel.CityId);
                return View(hotel);
            }
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Attraction)
                .Include(h => h.City)
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Find hotel with related bookings
                var hotel = await _context.Hotels
                    .Include(h => h.Bookings)
                    .FirstOrDefaultAsync(h => h.HotelId == id);

                if (hotel == null)
                {
                    return NotFound();
                }

                // First delete related bookings
                if (hotel.Bookings != null && hotel.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(hotel.Bookings);
                }

                // Then delete the hotel
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting hotel: {ex.Message}");
                ModelState.AddModelError("", "Error deleting hotel: " + ex.Message);

                // Reload the hotel with related entities for the view
                var hotel = await _context.Hotels
                    .Include(h => h.Attraction)
                    .Include(h => h.City)
                    .FirstOrDefaultAsync(h => h.HotelId == id);

                return View(hotel);
            }
        }
    }
}
