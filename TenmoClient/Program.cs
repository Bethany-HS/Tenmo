using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountService accountService = new AccountService();
        private static readonly TransferService transferService = new TransferService();
        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            Console.Clear();
            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    decimal balance = accountService.GetBalance(UserService.GetUserId());
                    Console.WriteLine($"Your account balance is: {balance:C2}");
                    Console.ReadLine();

                }
                else if (menuSelection == 2)
                {
                    ViewTransfers();

                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    SendMoney();
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }

        private static void ViewTransfers()
        {
            Console.Clear();
            List<ReturnTransfer> transfers = transferService.GetTransfers(UserService.GetUserId());
            if (transfers == null || transfers.Count == 0)
            {
                Console.WriteLine("Sorry we couldn't find any transfers");
                Console.WriteLine("Press any key to return");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.Write("ID");
            Console.Write("From/To".PadLeft(17));
            Console.WriteLine("Amount".PadLeft(17));
            Console.WriteLine("-------------------------------------------");
            
            foreach(ReturnTransfer transfer in transfers)
            {
                Console.Write($"{transfer.Transfer_id}");
                if(transfer.FromName == UserService.GetUserName())
                {
                    Console.Write("To: ".PadLeft(15));
                    Console.Write($"{transfer.ToName}".PadLeft(5));
                    Console.WriteLine($"{transfer.Amount:C2}".PadLeft(17));
                }
                else
                {
                    Console.Write("From: ".PadLeft(15));
                    Console.Write($"{transfer.FromName}");
                    Console.WriteLine($"{transfer.Amount:C2}".PadLeft(17));
                }

            }
            Console.WriteLine("---------");
            Console.Write("Please enter the transfer ID to view details (0 to cancel): ");
            string userInput = Console.ReadLine().Trim();
            bool validTransfer = false;
            if(userInput == "0")
            {
                return;
            }
            do
            {
                try
                {
                    ReturnTransfer returnTransfer = transferService.GetTransfer(Convert.ToInt32(userInput));
                    if (returnTransfer != null)
                    {
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine($"Id: {returnTransfer.Transfer_id}");
                        Console.WriteLine($"From: {returnTransfer.FromName}");
                        Console.WriteLine($"To: {returnTransfer.ToName}");
                        Console.WriteLine($"Type: {returnTransfer.Transfer_type_id}");
                        Console.WriteLine($"Status: {returnTransfer.Transfer_status_id}");
                        Console.WriteLine($"Amount: {returnTransfer.Amount:C2}");
                    }
                    else
                    {
                        Console.WriteLine("Please select a valid transfer.");
                    }

                }
                catch
                {
                    Console.WriteLine("Please select a valid transfer");
                }
            } while (!validTransfer);

        }

        private static void SendMoney()
        {
            bool selectedUser = false;
            string sendToUser = "";
            do
            {
                List<OtherUser> otherUsers = accountService.RetrieveUsers();
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("Users");
                Console.Write("ID");
                Console.WriteLine("Name".PadLeft(15));
                Console.WriteLine("-------------------------------------------");
                foreach (OtherUser user in otherUsers)
                {
                    if (user.User_Id != UserService.GetUserId())
                    {
                        Console.Write($"{user.User_Id}");
                        Console.WriteLine($"{user.Username}".PadLeft(15));
                    }
                }
                Console.WriteLine("---------\n");
                Console.Write("Enter ID of the user you are sending to (0 to cancel): ");
                sendToUser = Console.ReadLine().Trim();
                if(Convert.ToInt32(sendToUser) == 0)
                {
                    return;
                }
                try
                {
                    if (accountService.GetBalance(Convert.ToInt32(sendToUser)) == -1)
                    {
                        Console.WriteLine("Please enter a valid user");
                    }
                    else
                    {
                        selectedUser = true;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Please select a valid user ID");
                }

            } while (!selectedUser);
            bool moneySent = false;
            do
            {
                Console.Write("Enter the amount to send: ");
                string moneyToSend = Console.ReadLine().Trim();
                try
                {
                    decimal convertedMoneyToSend = Convert.ToDecimal(moneyToSend);
                    if(convertedMoneyToSend < 0 || convertedMoneyToSend == 0)
                    {
                        Console.WriteLine("Please enter a postive number");
                    }
                    else
                    {
                        if (convertedMoneyToSend > accountService.GetBalance(UserService.GetUserId()))
                        {
                            Console.WriteLine("Sorry, you don't have enough funds.");
                        }
                        else
                        {
                            Transfer transfer = new Transfer(2, 2, UserService.GetUserId(), Convert.ToInt32(sendToUser), convertedMoneyToSend);
                            if (transferService.SendMoney(transfer))
                            {
                                Console.WriteLine("Successfully sent, thank you for using TEnmo!");
                                Console.ReadLine();
                                moneySent = true;
                                Console.Clear();
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Something went wrong, please press a button to try again");
                                Console.ReadLine();
                            }
                        }

                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Sorry, that's not a valid entry, please press a button to try again");
                    Console.ReadLine();
                }

            } while (!moneySent);
        }
    }
}
