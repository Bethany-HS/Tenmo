﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService : BaseService
    {
        private readonly string API_BASE_URL = "https://localhost:44315/";
        private readonly RestClient client = new RestClient();

        public List<ReturnTransfer> GetTransfers(int userID)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"api/account/{userID}/transfers");
            IRestResponse<List<ReturnTransfer>> response = client.Get<List<ReturnTransfer>>(request);
            if (ProcessResponse(response))
            {
                return response.Data;
            }
            return null;
        }

        public bool SendMoney(Transfer transfer)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/transfers");
            request.AddJsonBody(transfer);
            IRestResponse response = client.Post(request);
            if (ProcessResponse(response))
            {
                return true;
            }
            else return false;
        }

        public ReturnTransfer GetTransfer(int transferId)
        {
            RestRequest request = new RestRequest(API_BASE_URL);
            IRestResponse<ReturnTransfer> response = client.Get<ReturnTransfer>(request);
            if (ProcessResponse(response))
            {
                return response.Data;
            }
            return null;
        }
    }
}
