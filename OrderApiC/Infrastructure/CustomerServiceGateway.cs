using RestSharp;
using SharedModels;

namespace OrderApiC.Infrastructure
{
    public class CustomerServiceGateway : IServiceGateway<CustomerDto>
    {
        string customerServiceBaseUrl;

        public CustomerServiceGateway(string baseUrl)
        {
            customerServiceBaseUrl = baseUrl;
        }

        public CustomerDto Get(Guid id)
        {

            RestClient c = new RestClient(customerServiceBaseUrl);

            var request = new RestRequest(id.ToString());
            var response = c.GetAsync<CustomerDto>(request);
            response.Wait();
            return response.Result;
        }
    }


}
