using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    
    public class TransferController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public TransferController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        [HttpGet]
        public ActionResult<decimal> GetMyBalance()
        {

            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            
            Account userAccount = _userDao.GetAccountBalance(userId);

            if (userAccount != null)
            {
                return Ok(userAccount.Balance);
            }
            return NotFound("Account not found");
        }

        [HttpGet("users")]
        public ActionResult<User> GetUsers()
        {
            List<User> userList = _userDao.GetUsers();

            if (userList != null)
            {
                return Ok(userList);
            }
            return NotFound("Users not found");
        }

        [HttpPut("transferamount")]
        public ActionResult<Transfer> SendTransfer(int toUserId, int fromUserId, decimal amount)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Transfer transfer = _userDao.SendATransfer(toUserId, fromUserId, amount);

            return Ok("Transfer Successful");
        }
     
    }
}
