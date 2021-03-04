using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient
{
    public class AccountService : BaseService
    {
        private readonly string API_BASE_URL = "https://localhost:44315/";
        private readonly RestClient client = new RestClient();

        public decimal GetBalance(int userID)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"api/account/{userID}/balance");
            IRestResponse<decimal> balance = client.Get<decimal>(request);
            if (ProcessResponse(balance))
            {
                return balance.Data;
            }
            return -1;

        }
    }
}
