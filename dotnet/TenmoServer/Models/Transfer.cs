using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        
        public int TransferId { get; set; }
        public decimal Amount { get; set; }
        public int TransferStatusId { get; set; }
        public int TransferTypeId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public string UserName { get; set; }
        public int AccountId { get; set; }
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }

    }
}
