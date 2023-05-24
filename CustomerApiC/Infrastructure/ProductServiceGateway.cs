﻿using RestSharp;
using SharedModels;

namespace CustomerApi.Infrastructure
{
    public class ProductServiceGateway : IServiceGateway<ProductDto>
    {
        string productServiceBaseUrl;

        public ProductServiceGateway(string baseUrl)
        {
            productServiceBaseUrl = baseUrl;
        }

        public ProductDto Get(Guid id)
        {
            RestClient c = new RestClient(productServiceBaseUrl);

            var request = new RestRequest(id.ToString());
            var response = c.GetAsync<ProductDto>(request);
            response.Wait();
            return response.Result;
        }
    }
}
