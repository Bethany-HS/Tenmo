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
        private TransferSqlDAO transferDAO;

        public AccountController(AccountDAO _accountDAO, IUserDAO _userDAO, TransferSqlDAO _transferDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
            transferDAO = _transferDAO;
        }

        [HttpGet("{id}/balance")]
        public ActionResult<Decimal> GetBalance(int id)
        {
            return Ok(accountDAO.GetBalance(id));
        }

        [HttpGet("/users")]
        public ActionResult<List<User>> GetOtherUsers()
        {
            return Ok(userDAO.GetUsers());
        }
        [HttpGet("{id}/transfers")]
        public ActionResult<List<ReturnTransfer>> GetAllTransfers(int id)
        {
            List<ReturnTransfer> transfers = transferDAO.GetTransfers(id);
            return transfers;
        }
        [HttpPost("transfers")]
        public ActionResult<Transfer> SendMoney(Transfer incomingTransfer)
        {
            Transfer result = transferDAO.SendMoney(incomingTransfer);
            return Created($"/transfers/{result.Transfer_id}", result);
        }
    }
}
