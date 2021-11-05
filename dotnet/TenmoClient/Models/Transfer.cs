using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    class Transfer
    {
        public int TransferId { get; set; }
        public decimal Amount { get; set; }
        public int TransferStatusId { get; set; }
        public int TransferTypeId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
    }
}
