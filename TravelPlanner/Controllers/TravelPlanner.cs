using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelPlanner.Data;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    [Authorize]
    public class TravelPlannerController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public TravelPlannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Step 1: Select a country
        public async Task<IActionResult> SelectCountry(string region = null, string currency = null, string language = null)
        {
            Console.WriteLine($"Filter values - Region: {region}, Currency: {currency}, Language: {language}");

            // Get all regions, currencies, and languages for filter dropdowns
            ViewBag.Regions = await _context.Countries.Select(c => c.Region).Distinct().Where(r => r != null).ToListAsync();
            ViewBag.Currencies = await _context.Countries.Select(c => c.Currency).Distinct().Where(c => c != null).ToListAsync();
            ViewBag.Languages = await _context.Countries.Select(c => c.Language).Distinct().Where(l => l != null).ToListAsync();

            // Start with all countries
            var countriesQuery = _context.Countries.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(region))
            {
                countriesQuery = countriesQuery.Where(c => c.Region == region);
            }

            if (!string.IsNullOrEmpty(currency))
            {
                countriesQuery = countriesQuery.Where(c => c.Currency == currency);
            }

            if (!string.IsNullOrEmpty(language))
            {
                countriesQuery = countriesQuery.Where(c => c.Language == language);
            }

            // Get the filtered countries
            var countries = await countriesQuery.ToListAsync();
            Console.WriteLine($"Found {countries.Count} countries after filtering");

            // Pass the selected values to the view
            ViewBag.SelectedRegion = region;
            ViewBag.SelectedCurrency = currency;
            ViewBag.SelectedLanguage = language;

            return View(countries);
        }

        // Step 2: Select attractions
        public async Task<IActionResult> SelectAttractions(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId);
            if (country == null)
            {
                return NotFound();
            }

            ViewBag.Country = country;
            ViewBag.Cities = await _context.Cities.Where(c => c.CountryId == countryId).ToListAsync();
            var attractions = await _context.Attractions
                .Where(a => a.CountryId == countryId)
                .Include(a => a.City)
                .ToListAsync();

            return View(attractions);
        }

        // Step 3: Select dates
        public IActionResult SelectDates(int countryId, string selectedAttractions)
        {
            ViewBag.CountryId = countryId;
            ViewBag.SelectedAttractions = selectedAttractions;
            return View();
        }

        // Step 4: Select hotel
        public async Task<IActionResult> SelectHotels(int countryId, string selectedAttractions, DateTime startDate, DateTime endDate)
        {
            // Parse the selected attractions
            var attractionIds = selectedAttractions.Split(',').Select(int.Parse).ToList();

            // Get hotels near the selected attractions
            var hotels = await _context.Hotels
                .Where(h => attractionIds.Contains(h.AttractionId))
                .Include(h => h.Attraction)
                .Include(h => h.City)
                .ToListAsync();

            ViewBag.CountryId = countryId;
            ViewBag.SelectedAttractions = selectedAttractions;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(hotels);
        }

        // Complete booking
        [HttpPost]
        public async Task<IActionResult> CompleteBooking(int countryId, string selectedAttractions,
            DateTime startDate, DateTime endDate, int? hotelId)
        {
            // For simplicity, we'll use a fixed user ID
            int userId = 1;

            // Parse attraction IDs
            var attractionIds = selectedAttractions.Split(',').Select(int.Parse).ToList();

            // Create a booking for each attraction
            foreach (var attractionId in attractionIds)
            {
                var booking = new Booking
                {
                    UserId = userId,
                    CountryId = countryId,
                    AttractionId = attractionId,
                    HotelId = hotelId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalPrice = CalculateTotalPrice(startDate, endDate, attractionId, hotelId)
                };

                _context.Bookings.Add(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("BookingConfirmation", new { userId });
        }

        // Calculate total price
        private decimal CalculateTotalPrice(DateTime startDate, DateTime endDate, int attractionId, int? hotelId)
        {
            decimal totalPrice = 0;

            // Calculate the number of nights
            int nights = (int)(endDate - startDate).TotalDays;

            // Add hotel price if a hotel was selected
            if (hotelId.HasValue)
            {
                var hotel = _context.Hotels.Find(hotelId.Value);
                if (hotel != null)
                {
                    totalPrice += hotel.Price * nights;
                }
            }

            // Optionally add attraction prices or other costs
            var attraction = _context.Attractions.Find(attractionId);
            if (attraction != null)
            {
                // Add a placeholder cost based on budget level
                switch (attraction.BudgetLevel?.ToLower())
                {
                    case "high":
                        totalPrice += 50;
                        break;
                    case "medium":
                        totalPrice += 30;
                        break;
                    case "low":
                        totalPrice += 15;
                        break;
                    default:
                        totalPrice += 20;
                        break;
                }
            }

            return totalPrice;
        }

        // Booking confirmation
        public async Task<IActionResult> BookingConfirmation(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Country)
                .Include(b => b.Attraction)
                .ThenInclude(a => a.City)
                .Include(b => b.Hotel)
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();

            return View(bookings);
        }
        
        // View all user bookings
        public async Task<IActionResult> MyBookings(int userId = 1) // Default user ID for now
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Country)
                .Include(b => b.Attraction)
                .ThenInclude(a => a.City)
                .Include(b => b.Hotel)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            return View(bookings);
        }
    }
}