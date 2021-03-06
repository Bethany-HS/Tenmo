using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AccountService : BaseService
    {
        private readonly string API_BASE_URL = "https://localhost:44315/";
        private readonly RestClient client = new RestClient();

        public decimal GetBalance(int userID)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"api/account/{userID}/balance");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<decimal> balance = client.Get<decimal>(request);
            if (ProcessResponse(balance))
            {
                return balance.Data;
            }
            return -1;

        }

        public List<OtherUser> RetrieveUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "users");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<OtherUser>> users = client.Get<List<OtherUser>>(request);

            return users.Data;
        }
    }
}
