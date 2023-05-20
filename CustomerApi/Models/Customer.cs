﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerId { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        public bool CreditStanding { get; set; }
    }
}
