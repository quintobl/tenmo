using System;
using System.Collections.Generic;
using TenmoClient.Models;
using RestSharp;

namespace TenmoClient
{
    public class ConsoleService
    {
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public void PrintBalance(decimal balance)
        {
            Console.WriteLine($"Your current account balance is: ${balance}");
        }

        public void PrintUsers(List<User> users)
        {
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine("ID          Name");
            Console.WriteLine("----------------------------------------------");

            foreach (User user in users)
            {
                Console.WriteLine(user.UserId + "         " + user.Username);
            }
            
            Console.WriteLine("------------");
            Console.WriteLine("Enter ID of user you are sending to (0 to cancel): ");
            Console.WriteLine("Enter amount: ");

        }

        public void PrintTransferSuccess(string transfer)
        {
            if (transfer != null)
            {
                Console.WriteLine("Transfer was successful");
            }
        }

        public void PrintTransferList(List<Transfer> transfers)
        {
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID               From/To                Amount");
            Console.WriteLine("----------------------------------------------");
            foreach (Transfer transfer in transfers)
            {
                string to = "To: ";
                string from = "From: ";
                
                //string userName = Convert.ToString(user.Username);


                if (transfer.TransferTypeId == 2)
                {
                    Console.WriteLine(transfer.TransferId + "         " + 
                        to + Environment.UserName + "         " + "$" + transfer.Amount);
                } else if (transfer.TransferTypeId == 1)
                {
                    Console.WriteLine(transfer.TransferId + "         " +
                        from + Environment.UserName + "         " + "$" + transfer.Amount);
                }
                
            }
        }


    }
}
