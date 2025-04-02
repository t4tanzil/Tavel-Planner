using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TravelPlanner.Models
    {
        public class Booking
        {
            [Key]
            public int BookingId { get; set; }

            public String UserId { get; set; }

            [ForeignKey("Country")]
            public int CountryId { get; set; }

            [ForeignKey("Attraction")]
            public int AttractionId { get; set; }

            [ForeignKey("Hotel")]
            public int? HotelId { get; set; }  // Optional

            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            [Column(TypeName = "decimal(10, 2)")]
            public decimal TotalPrice { get; set; }

            // Navigation properties
            public virtual Country Country { get; set; }
            public virtual Attraction Attraction { get; set; }
            public virtual Hotel Hotel { get; set; }
        }
    }