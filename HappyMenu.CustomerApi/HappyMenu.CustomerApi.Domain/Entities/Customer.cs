﻿using System;

namespace HappyMenu.CustomerApi.Domain.Entities
{
    public partial class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Age { get; set; }
    }
}
