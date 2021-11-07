using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDao
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        Account GetAccountBalance(int userId);
        Transfer SendATransfer(int toUserId, int fromUserId, decimal amount);
        List<Transfer> ViewAllTransfers(int userId);
        public Transfer GetSingleTransfer(int transferId);
    }
}
