 using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TravelPlanner.Models
    {
        public class Hotel
        {
            [Key]
            public int HotelId { get; set; }

            [ForeignKey("Attraction")]
            public int AttractionId { get; set; }

            [ForeignKey("City")]
            public int CityId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [Range(1, 5)]
            public int Stars { get; set; }

            [Column(TypeName = "decimal(10, 2)")]
            public decimal Price { get; set; }

            public string Address { get; set; }

            public string Contact { get; set; }

            // Navigation properties
            public virtual Attraction Attraction { get; set; }
            public virtual City City { get; set; }
            public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        }
    }

