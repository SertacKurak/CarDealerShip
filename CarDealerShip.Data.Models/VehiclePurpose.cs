﻿using System.ComponentModel.DataAnnotations;

namespace CarDealership.Data.Models
{
    using static Common.Constants.PurposeValidations;
    public class VehiclePurpose
    {
        public Guid Id { get; set; }

        [Required] 
        [MaxLength(NameMaxLength)] 
        public string Name { get; set; } = null!;

        public List<Vehicle> Vehicles { get; set; }
            = new List<Vehicle>();
    }
}
