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
            User user = _userDao.GetUser(User.Identity.Name);
            Account userAccount = _userDao.GetAccountBalance();

            if (user != null) 
            {
                return Ok(userAccount.Balance);
            }
            return NotFound("Account not found");
        }



     
    }
}
