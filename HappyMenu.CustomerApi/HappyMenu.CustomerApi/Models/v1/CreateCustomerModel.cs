﻿using System.ComponentModel.DataAnnotations;
using System;

namespace HappyMenu.CustomerApi.Models.v1
{
    public class CreateCustomerModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime? Birthday { get; set; }

        public int? Age { get; set; }
    }
}