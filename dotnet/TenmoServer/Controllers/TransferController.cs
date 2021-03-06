using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TenmoServer.DAO;
using TenmoServer.Models;

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
        public ActionResult<Account> GetMyBalance()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Account userAccount = _userDao.GetAccountBalance(userId);

            if (userAccount != null)
            {
                return Ok(userAccount);
            }
            return NotFound("Account not found");
        }

        [HttpGet("users")]
        public ActionResult<List<User>> GetUsers()
        {
            List<User> userList = _userDao.GetUsers();

            if (userList != null)
            {
                return Ok(userList);
            }
            return NotFound("Users not found");
        }


        [HttpPut("transferamount/{toUserId}/{amount}")]
        public ActionResult<Transfer> SendTransfer(int toUserId, decimal amount)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Transfer transfer = _userDao.SendATransfer(toUserId, userId, amount);

            return Ok("Transfer Successful");
        }


        [HttpGet("list")]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            
            List<Transfer> transferList = _userDao.ViewAllTransfers(userId);

            if (transferList != null)
            {
                return Ok(transferList);
            }
            return NotFound("Transfers not found");
        }


        [HttpGet("single/{transferId}")]
        public ActionResult<Transfer> GetSingleTransfer(int transferId)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Transfer transfer = _userDao.GetSingleTransfer(transferId);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            return NotFound("Transfers not found");
        }

    }
}
