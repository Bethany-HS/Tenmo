using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountDAO accountDAO;
        private IUserDAO userDAO;

        public AccountController(AccountDAO _accountDAO, IUserDAO _userDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
        }

       [HttpGet("{id}/balance")]
       public ActionResult<Decimal> GetBalance(int id)
        {
                return Ok(accountDAO.GetBalance(id));
        }

    }
}
