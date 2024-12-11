﻿using System.ComponentModel.DataAnnotations;

namespace CarDealership.Data.Models
{
    using static Common.Constants.LocationValidations;
    public class Location
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Vehicle> Vehicles { get; set; } =
            new HashSet<Vehicle>();
    }
}
