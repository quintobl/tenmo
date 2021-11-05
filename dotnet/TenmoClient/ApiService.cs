using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Exceptions;
using TenmoClient.Models;

namespace TenmoClient
{
    public class ApiService
    {
        private readonly string API_URL = "";
        private readonly RestClient client = new RestClient();
        private ApiUser user = new ApiUser();

        public bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }

        public ApiService(string api_url)
        {
            API_URL = api_url;
        }

        public decimal GetBalance()
        {
            RestRequest request = new RestRequest(API_URL + "transfer");
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return response.Data;
        }

        private void ProcessErrorResponse(IRestResponse<decimal> response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new NoResponseException("Error occurred - unable to reach server.", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new NonSuccessException((int)response.StatusCode);
            }
        }
    }
}
