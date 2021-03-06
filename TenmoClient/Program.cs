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
            bool showBalance = false;
            bool inMenu = true;
            int menuSelection = -1;
            while (inMenu)
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
                if (showBalance == true)
                {
                    decimal balance = accountService.GetBalance(UserService.GetUserId());
                    Console.WriteLine($"Your account balance is: {balance:C2}");
                }
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    showBalance = true;
                    Console.Clear();
                }
                else if (menuSelection == 2)
                {
                    ViewTransfers();

                }
                else if (menuSelection == 3)
                {
                    GetPendingTransfers();
                }
                else if (menuSelection == 4)
                {
                    SendMoney();
                }
                else if (menuSelection == 5)
                {
                    RequestTransfer();
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else if (menuSelection == 0)
                {
                    Console.WriteLine("Goodbye!");
                    inMenu = false;
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
                Console.WriteLine("Press enter to return");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.Write("ID");
            Console.Write("From/To".PadLeft(17));
            Console.WriteLine("Amount".PadLeft(17));
            Console.WriteLine("-------------------------------------------");

            foreach (ReturnTransfer transfer in transfers)
            {
                if (transfer.Transfer_status_id == 2)
                {
                    Console.Write($"{transfer.Transfer_id}".PadRight(10));
                    if (transfer.FromName == UserService.GetUserName())
                    {
                        Console.Write("To: ".PadRight(6));
                        Console.Write($"{transfer.ToName}");
                        Console.WriteLine($"{transfer.Amount:C2}".PadLeft(15));
                    }
                    else
                    {
                        Console.Write("From: ");
                        Console.Write($"{transfer.FromName}");
                        Console.WriteLine($"{transfer.Amount:C2}".PadLeft(15));
                    }
                }


            }
            Console.WriteLine("---------");
            bool validTransfer = false;
            do
            {
                Console.Write("Please enter the transfer ID to view details (0 to cancel): ");
                string userInput = Console.ReadLine().Trim();

                if (userInput == "0")
                {
                    Console.Clear();
                    return;
                }
                try
                {
                    ReturnTransfer returnTransfer = transferService.GetTransfer(Convert.ToInt32(userInput));
                    if (returnTransfer.Transfer_id != 0)
                    {
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine("Transfer Details");
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine($"Id: {returnTransfer.Transfer_id}");
                        Console.WriteLine($"From: {returnTransfer.FromName}");
                        Console.WriteLine($"To: {returnTransfer.ToName}");
                        Console.WriteLine($"Type: {returnTransfer.TransferType}");
                        Console.WriteLine($"Status: {returnTransfer.TransferStatus}");
                        Console.WriteLine($"Amount: {returnTransfer.Amount:C2}");
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine("Please press enter to return");
                        Console.ReadLine();
                        Console.Clear();
                        validTransfer = true;
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
            string sendToUser;
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
                if (Convert.ToInt32(sendToUser) == 0)
                {
                    Console.Clear();
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
                    if (convertedMoneyToSend < 0 || convertedMoneyToSend == 0)
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
                                Console.WriteLine("Please press enter to return");
                                Console.ReadLine();
                                moneySent = true;
                                Console.Clear();
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Something went wrong, please press enter to try again");
                                Console.ReadLine();
                            }
                        }

                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Sorry, that's not a valid entry, please press enter to try again");
                    Console.ReadLine();
                }

            } while (!moneySent);
        }

        private static void RequestTransfer()
        {
            bool selectedUser = false;
            string requestFromUser;
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
                Console.Write("Enter ID of the user you are requesting from (0 to cancel): ");
                requestFromUser = Console.ReadLine().Trim();
                if (Convert.ToInt32(requestFromUser) == 0)
                {
                    Console.Clear();
                    return;
                }
                try
                {
                    if (accountService.GetBalance(Convert.ToInt32(requestFromUser)) == -1)
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
            bool requestSent = false;
            do
            {
                Console.Write("Enter the amount to request: ");
                string moneyToSend = Console.ReadLine().Trim();
                try
                {
                    decimal convertedMoneyToSend = Convert.ToDecimal(moneyToSend);
                    if (convertedMoneyToSend < 0 || convertedMoneyToSend == 0)
                    {
                        Console.WriteLine("Please enter a postive number");
                    }
                    else
                    {
                        Transfer transfer = new Transfer(1, 1, Convert.ToInt32(requestFromUser), UserService.GetUserId(), convertedMoneyToSend);
                        if (transferService.SendMoney(transfer))
                        {
                            Console.WriteLine("Successfully requested, the user will need to accept before you can access the funds.");
                            Console.WriteLine("Please press enter to return");
                            Console.ReadLine();
                            requestSent = true;
                            Console.Clear();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong, please press enter to try again");
                            Console.ReadLine();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Sorry, that's not a valid entry, please press enter to try again");
                    Console.ReadLine();
                }
            } while (!requestSent);
        }

        private static void GetPendingTransfers()
        {
            bool transferSelected = false;
            do
            {
                Console.Clear();
                List<ReturnTransfer> transfers = transferService.GetTransfers(UserService.GetUserId());
                if (transfers == null || transfers.Count == 0)
                {
                    Console.WriteLine("Sorry we couldn't find any transfers");
                    Console.WriteLine("Press enter to return");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("Transfers");
                Console.Write("ID");
                Console.Write("To".PadLeft(17));
                Console.WriteLine("Amount".PadLeft(17));
                Console.WriteLine("-------------------------------------------");

                foreach (ReturnTransfer transfer in transfers)
                {
                    if (transfer.Transfer_status_id == 1 && transfer.Account_from == UserService.GetUserId())
                    {
                        Console.Write($"{transfer.Transfer_id}".PadRight(10));
                        Console.Write("To: ".PadRight(6));
                        Console.Write($"{transfer.ToName}");
                        Console.WriteLine($"{transfer.Amount:C2}".PadLeft(15));
                    }
                }
                Console.WriteLine("---------");
                Console.Write("Please enter transfer ID to approve/reject (0 to cancel): ");
                string approveRejectTransferChoice = Console.ReadLine().Trim();
                try
                {
                    if (Convert.ToInt32(approveRejectTransferChoice) == 0)
                    {
                        return;
                    }
                    ReturnTransfer approveRejectTransfer = transferService.GetTransfer(Convert.ToInt32(approveRejectTransferChoice));
                    if (approveRejectTransfer.Transfer_id != 0 && approveRejectTransfer.Transfer_status_id == 1
                        && approveRejectTransfer.Account_from == UserService.GetUserId() && approveRejectTransfer.Amount
                        <= accountService.GetBalance(UserService.GetUserId()))
                    {
                        Console.WriteLine("1: Approve");
                        Console.WriteLine("2: Reject");
                        Console.WriteLine("3: Don't approve or reject");
                        Console.WriteLine("---------");
                        Console.Write("Please choose an option: ");
                        string approveReject = Console.ReadLine();
                        switch (approveReject)
                        {
                            case "1":
                                ;
                                approveRejectTransfer.Transfer_status_id = 2;
                                transferService.UpdateTransferStatus(approveRejectTransfer);
                                transferSelected = true;
                                Console.WriteLine("Transfer Approved");
                                break;
                            case "2":
                                approveRejectTransfer.Transfer_status_id = 3;
                                transferService.UpdateTransferStatus(approveRejectTransfer);
                                transferSelected = true;
                                Console.WriteLine("Transfer Rejected");
                                break;
                            case "3":
                                transferSelected = true;
                                return;
                            default:
                                return;
                        }
                    }
                    else if (approveRejectTransfer.Amount > accountService.GetBalance(UserService.GetUserId()))
                    {
                        Console.WriteLine("You don't have enough funds to fulfill this request.");
                    }
                    else
                    {
                        Console.WriteLine("Please choose a valid transfer");
                    }
                }
                catch
                {
                    Console.WriteLine("Please enter a valid option");
                }

                
            } while (!transferSelected);
        }
    }
}
