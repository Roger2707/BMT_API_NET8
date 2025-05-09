﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public Guid BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PercentageDiscount { get; set; }
    }
}
