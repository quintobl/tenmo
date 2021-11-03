using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TenmoServer.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }

    }
}
