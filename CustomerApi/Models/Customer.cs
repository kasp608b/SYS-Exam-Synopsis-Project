﻿namespace CustomerApi.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        public bool CreditStanding { get; set; }
    }
}
