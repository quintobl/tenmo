using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;
using System.Security.Claims;

namespace TenmoServer.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(username);
        }

        public Account GetAccountBalance(int userId)
        {
            Account account = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance, account_id FROM accounts JOIN users ON accounts.user_id = users.user_id " +
                                                        "WHERE accounts.user_id = @userid", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        account.Balance = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return account;
        }


        public Transfer SendATransfer(int toUserId, int fromUserId, decimal amount)
        {

            Transfer transfer = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)" +
                                                        $"VALUES(2, 2, (SELECT accounts.account_id FROM accounts WHERE user_id = { fromUserId })," +
                                                        $"(SELECT accounts.account_id FROM accounts WHERE user_id = { toUserId}), { amount})", conn);
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();


                    cmd = new SqlCommand("UPDATE accounts " +
                                            $"SET accounts.balance = (accounts.balance - {amount}) " +
                                            $"WHERE accounts.user_id = {fromUserId}", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    cmd.ExecuteNonQuery();


                    cmd = new SqlCommand("UPDATE accounts " +
                                            $"SET accounts.balance = (accounts.balance + {amount}) " +
                                            $"WHERE accounts.user_id = {toUserId}", conn);
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();


                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int transferId = Convert.ToInt32(cmd.ExecuteScalar());
                    transfer.AccountFrom = fromUserId;
                    transfer.AccountTo = toUserId;
                    transfer.Amount = amount;
                    transfer.TransferId = transferId;
                    transfer.TransferStatusId = 2;
                    transfer.TransferTypeId = 2;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            
            return transfer;
        }



        public List<Transfer> ViewAllTransfers(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfers.transfer_id, transfers.transfer_type_id, transfers.transfer_status_id, transfers.account_from, transfers.account_to, transfers.amount, users.username, accounts.account_id " +
                                                        "FROM transfers " +
                                                        "JOIN accounts ON accounts.account_id = transfers.account_from OR accounts.account_id = transfers.account_to " +
                                                        "JOIN users ON accounts.user_id = users.user_id " +
                                                        "WHERE users.user_id = @userid", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader1 = cmd.ExecuteReader();


                    //cmd = new SqlCommand($"SELECT TOP 1 users.username " +
                    //                        "FROM users " +
                    //                        "JOIN accounts ON accounts.user_id = users.user_id " +
                    //                        "JOIN transfers ON transfers.account_from = accounts.account_id", conn);
                    ////cmd.Parameters.AddWithValue("@username", username);
                    //cmd.ExecuteNonQuery();
                    //SqlDataReader reader2 = cmd.ExecuteReader();


                    //cmd = new SqlCommand("SELECT TOP 1 users.username " +
                    //                        "FROM users " +
                    //                        "JOIN accounts ON accounts.user_id = users.user_id " +
                    //                        "JOIN transfers ON transfers.account_to = accounts.account_id", conn);
                    ////cmd.Parameters.AddWithValue("@username", username);
                    //cmd.ExecuteNonQuery();
                    //SqlDataReader reader3 = cmd.ExecuteReader();


                    while (reader1.Read())
                    {
                        Transfer t = GetTransfersFromReader1(reader1);
                        returnTransfers.Add(t);
                    }

                    //while (reader2.Read())
                    //{
                    //    Transfer t = GetTransfersFromReader2(reader2);
                    //    returnTransfers.Add(t);
                    //}

                    //while (reader3.Read())
                    //{
                    //    Transfer t = GetTransfersFromReader3(reader3);
                    //    returnTransfers.Add(t);
                    //}
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnTransfers;
        }



        public Transfer GetSingleTransfer(int transferId)
        {
            Transfer returnTransfer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfers.transfer_id, transfers.transfer_type_id, " +
                                                    "transfers.transfer_status_id, transfers.account_from," +
                                                    " transfers.account_to, transfers.amount, users.username, " +
                                                    "accounts.account_id " +
                                                    "FROM transfers " +
                                                    "JOIN accounts ON accounts.account_id = transfers.account_from " +
                                                    "OR accounts.account_id = transfers.account_to" +
                                                    "JOIN users ON accounts.user_id = users.user_id " +
                                                    $"WHERE transfers.transfer_id = {transferId}", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnTransfer = GetTransfersFromReader1(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnTransfer;
        }



        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };
            return u;
        }

        private decimal GetAccountFromReader(SqlDataReader reader)
        {
            Account a = new Account()
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["balance"]),

            };

            return a.Balance;
        }

        private Transfer GetTransfersFromReader1(SqlDataReader reader1)
        {
            Transfer t = new Transfer();
            {
                t.TransferId = Convert.ToInt32(reader1["transfer_id"]);
                t.Amount = Convert.ToDecimal(reader1["amount"]);
                t.TransferStatusId = Convert.ToInt32(reader1["transfer_status_id"]);
                t.TransferTypeId = Convert.ToInt32(reader1["transfer_type_id"]);
                t.AccountFrom = Convert.ToInt32(reader1["account_from"]);
                t.AccountTo = Convert.ToInt32(reader1["account_to"]);
                t.AccountId = Convert.ToInt32(reader1["account_id"]);
                t.UserName = Convert.ToString(reader1["username"]);
                t.ToUserName = Convert.ToString(reader1["username"]);
                t.FromUserName = Convert.ToString(reader1["username"]);

            };
            return t;
        }

        //private Transfer GetTransfersFromReader2(SqlDataReader reader2)
        //{
        //    Transfer t = new Transfer();
        //    {
        //        t.TransferId = Convert.ToInt32(reader2["transfer_id"]);
        //        t.Amount = Convert.ToDecimal(reader2["amount"]);
        //        t.TransferStatusId = Convert.ToInt32(reader2["transfer_status_id"]);
        //        t.TransferTypeId = Convert.ToInt32(reader2["transfer_type_id"]);
        //        t.AccountFrom = Convert.ToInt32(reader2["account_from"]);
        //        t.AccountTo = Convert.ToInt32(reader2["account_to"]);
        //        t.AccountId = Convert.ToInt32(reader2["account_id"]);
        //        t.UserName = Convert.ToString(reader2["username"]);
        //        t.ToUserName = Convert.ToString(reader2["username"]);
        //        t.FromUserName = Convert.ToString(reader2["username"]);

        //    };
        //    return t;
        //}


        //private Transfer GetTransfersFromReader3(SqlDataReader reader3)
        //{
        //    Transfer t = new Transfer();
        //    {
        //        t.TransferId = Convert.ToInt32(reader3["transfer_id"]);
        //        t.Amount = Convert.ToDecimal(reader3["amount"]);
        //        t.TransferStatusId = Convert.ToInt32(reader3["transfer_status_id"]);
        //        t.TransferTypeId = Convert.ToInt32(reader3["transfer_type_id"]);
        //        t.AccountFrom = Convert.ToInt32(reader3["account_from"]);
        //        t.AccountTo = Convert.ToInt32(reader3["account_to"]);
        //        t.AccountId = Convert.ToInt32(reader3["account_id"]);
        //        t.UserName = Convert.ToString(reader3["username"]);
        //        t.ToUserName = Convert.ToString(reader3["username"]);
        //        t.FromUserName = Convert.ToString(reader3["username"]);

        //    };
        //    return t;
        //}



    }
}
