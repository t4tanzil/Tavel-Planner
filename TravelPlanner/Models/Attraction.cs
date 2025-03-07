using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TravelPlanner.Models
    {
        public class Attraction
        {
            [Key]
            public int AttractionId { get; set; }

            [ForeignKey("Country")]
            public int CountryId { get; set; }

            [ForeignKey("City")]
            public int CityId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [StringLength(50)]
            public string Type { get; set; }

            [Range(0, 5)]
            public float Rating { get; set; }

            [StringLength(50)]
            public string BudgetLevel { get; set; }

            public string Description { get; set; }

            // Navigation properties
            public virtual Country Country { get; set; }
            public virtual City City { get; set; }
            public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
            public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        }
    }

