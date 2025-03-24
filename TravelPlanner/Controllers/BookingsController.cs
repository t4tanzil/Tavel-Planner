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
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Attraction)
                .Include(b => b.Country)
                .Include(b => b.Hotel)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Attraction)
                .Include(b => b.Country)
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name");
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,UserId,CountryId,AttractionId,HotelId,StartDate,EndDate,TotalPrice")] Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", booking.AttractionId);
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", booking.CountryId);
                ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "Name", booking.HotelId);
                return View(booking);
            }
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", booking.AttractionId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", booking.CountryId);
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "Name", booking.HotelId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,UserId,CountryId,AttractionId,HotelId,StartDate,EndDate,TotalPrice")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewData["AttractionId"] = new SelectList(_context.Attractions, "AttractionId", "Name", booking.AttractionId);
                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", booking.CountryId);
                ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "Name", booking.HotelId);
                return View(booking);
            }
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Attraction)
                .Include(b => b.Country)
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound();
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting booking: {ex.Message}");
                ModelState.AddModelError("", "Error deleting booking: " + ex.Message);
                var booking = await _context.Bookings
                    .Include(b => b.Attraction)
                    .Include(b => b.Country)
                    .Include(b => b.Hotel)
                    .FirstOrDefaultAsync(m => m.BookingId == id);
                return View(booking);
            }
        }
    }
}