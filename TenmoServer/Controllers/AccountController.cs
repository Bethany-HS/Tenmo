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
        private UserSqlDAO userDAO;

        public AccountController(AccountDAO _accountDAO, UserSqlDAO _userDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
        }

       [HttpGet("balance")]
       public ActionResult<Decimal> GetBalance(ReturnUser user)
        {
            if (!string.IsNullOrEmpty(userDAO.GetUser(user.Username).Username))
            {
                return Ok(accountDAO.GetBalance(user.UserId));
            }
            return NotFound();
        }

    }
}
