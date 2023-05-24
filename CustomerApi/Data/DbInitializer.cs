using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Customers
            if (context.Customers.Any())
            {
                return; // DB has been seeded
            }

            /*
            List<Customer> customers = new List<Customer>
            {
                new Customer { CustomerId = Guid.Parse("2749cb35-1664-4c53-aa45-aff486bedf39"), CompanyName = "Zboncak", Email = "ZboncakMarquardtandWeber.Rippin@hotmail.com" , Phone = "645.786.1631 x0930" , BillingAddress = "03599 Kevon Courts, South Tre, Equatorial Guinea ", ShippingAddress = "32303 Nelda Knoll, Guidotown, Andorra",RegistrationNumber = "1111", CreditStanding = true },
            };

            FakeCustomerGenerator generator = new FakeCustomerGenerator();


            context.Customers.AddRange(customers);
            context.Customers.AddRange(generator.generateCustomers(10));
            context.SaveChanges();
            */
        }
    }
}
