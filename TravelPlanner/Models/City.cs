 using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TravelPlanner.Models
    {
        public class City
        {
            [Key]
            public int CityId { get; set; }

            [ForeignKey("Country")]
            public int CountryId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            public bool IsCapital { get; set; }

            // Navigation properties
            public virtual Country Country { get; set; }
            public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
            public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        }
    }
