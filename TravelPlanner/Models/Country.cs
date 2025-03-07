
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace TravelPlanner.Models
    {
        public class Country
        {
            [Key]
            public int CountryId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [StringLength(50)]
            public string Region { get; set; }

            [StringLength(50)]
            public string Currency { get; set; }

            [StringLength(50)]
            public string Language { get; set; }

            // Navigation properties
            public virtual ICollection<City> Cities { get; set; } = new List<City>();
            public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
            public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        }
    }
