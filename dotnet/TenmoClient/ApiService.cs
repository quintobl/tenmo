using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Exceptions;
using TenmoClient.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TenmoClient
{
    public class ApiService
    {
        private readonly string API_URL = "";
        private readonly RestClient client = new RestClient();
        private ApiUser user = new ApiUser();
        private Transfer transfer = new Transfer();

        //public bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }
        
        public ApiService(string api_url)
        {
            API_URL = api_url;
        }


        public Account GetBalance()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_URL + "transfer");
            IRestResponse<Account> response = client.Get<Account>(request);
         
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

        public List<User> GetUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_URL + "transfer" + "/" + "users");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

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

        public string MakeTransfer(int userId, decimal amountToTransfer)
        {
            Transfer transfer = new Transfer();
            transfer.AccountFrom = userId;
            transfer.Amount = amountToTransfer;
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_URL + "transfer" + "/" + "transferamount" + "/" + transfer.AccountFrom + "/" + transfer.Amount);
            request.AddJsonBody(transfer);
            IRestResponse<string> response = client.Put<string>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<Transfer> GetTransfers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_URL + "transfer" + "/" + "list");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

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



        private void ProcessErrorResponse(IRestResponse response)
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
