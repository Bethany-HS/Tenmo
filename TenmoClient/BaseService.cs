using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient
{
    public abstract class BaseService
    {
        public bool ProcessResponse(IRestResponse response)
        {
            if(response.ResponseStatus != ResponseStatus.Completed)
            {
                return false; 
            }else if (!response.IsSuccessful)
            {
                return false;
            }
            return true;
        }
    }
}
