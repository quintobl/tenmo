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


    }
}
