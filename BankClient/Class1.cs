using Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BankClient
{
    public class Client
    {
        #region data
            static String accountID = null;
            static String pesel;
            static accountType accType = new accountType();
            const String ip = "192.168.43.239";
            static Ice.Communicator ic = null;
        #endregion data

        public static PersonalData createPersonalData(String firstname, String lastname, String nationalIDNumber)
        {
            PersonalData personalData = new PersonalData();
            personalData.firstName = firstname;
            personalData.lastName = lastname;
            personalData.NationalIDNumber = nationalIDNumber;
            return personalData;
        }

        public static void createAccount()
        {
            String Fristname = null;
            String Lastname = null;
            String Type = null;

            Console.WriteLine("Give Personal Data");

            Console.WriteLine("Give Firstname:");
            Fristname = Console.ReadLine();

            Console.WriteLine("Give Lastname:");
            Lastname = Console.ReadLine();

            Console.WriteLine("Give Pesel:");
            pesel = Console.ReadLine();

            Console.WriteLine("Account Type (S/P):");

            Type = Console.ReadLine();

            if (Type == "P")
                accType = accountType.PREMIUM;
            else
                accType = accountType.SILVER;

            Ice.ObjectPrx obj = ic.stringToProxy("bankManager:ssl -h " + ip +  " -p 10001");
            BankManagerPrx bankManager = BankManagerPrxHelper.checkedCast(obj);

            if (bankManager == null)
                throw new ApplicationException("Invalid manager proxy");

            accountID = "";
            PersonalData myData = createPersonalData(Fristname, Lastname, pesel);
            bankManager.createAccount(myData, accType, out accountID);

            Console.WriteLine("YOUR ACCOUNT ID DO NOT LOSE IT!:  " + accountID);
            Console.WriteLine("8==>");
        }

        public static void transfer()
        {
            String tmpAccountID;
            if (accountID == null)
            {
                Console.WriteLine("Set your account number");
                tmpAccountID = Console.ReadLine();
            }
            else
            {
                tmpAccountID = accountID;
            }

            String accountNumber = null;
            String amount = null;

            Ice.ObjectPrx obj = ic.stringToProxy(tmpAccountID + ":ssl -h "  + ip +  " -p 10001");
            AccountPrx account = AccountPrxHelper.checkedCast(obj);

            if (account == null)
                throw new ApplicationException("Invalid account proxy");

            Console.WriteLine("Give Account Number:");
            accountNumber = Console.ReadLine();

            Console.WriteLine("Give Amount:");
            amount = Console.ReadLine();

            account.transfer(accountNumber, Convert.ToInt32(amount));

            Console.WriteLine("8==>");
        }

        public static void delete()
        {
            String accountNumber = null;
            Dictionary<String, String> dict = new Dictionary<String, String>();
            dict.Add("pesel",pesel);

            Ice.ObjectPrx obj = ic.stringToProxy("bankManager:ssl -h"  + ip +  " -p 10001");
            BankManagerPrx bankManager = BankManagerPrxHelper.checkedCast(obj);

            if (bankManager == null)
                throw new ApplicationException("Invalid manager proxy");

            Console.WriteLine("Give Account Number:");
            accountNumber = Console.ReadLine();

            bankManager.removeAccount(accountNumber, dict);

            Console.WriteLine("8==>");
        }

        public static void loan()
        {
            if (accType != accountType.PREMIUM)
            {
                Console.WriteLine(accType);
                Console.WriteLine("You need to have PREMIUM account to do that");
                return;
            }

            String tmpAccountID;
            if (accountID == null)
            {
                Console.WriteLine("Set your account number");
                tmpAccountID = Console.ReadLine();
            }
            else
            {
                tmpAccountID = accountID;
            }

            String accountNumber = null;
            int amount;
            currency curr;
            String currString;
            int period;
            int totalCost;
            float interestRate;

            Ice.ObjectPrx obj = ic.stringToProxy(accountID + ":ssl -h "  + ip +  " -p 10001");
            PremiumAccountPrx account = PremiumAccountPrxHelper.checkedCast(obj);

            if (account == null)
                throw new ApplicationException("Invalid account proxy");

            Console.WriteLine("Give Account Number:");
            accountNumber = Console.ReadLine();

            Console.WriteLine("Give Amount:");
            amount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Give Period:");
            period = Convert.ToInt32( Console.ReadLine() );

            Console.WriteLine("Give current:");
            currString = Console.ReadLine();

            if (currString.Equals("USD"))
                curr = currency.USD;
            else if (currString.Equals("EUR"))
                curr = currency.EUR;
            else if (currString.Equals("USD"))
                curr = currency.USD;
            else
                curr = currency.PLN;

            account.calculateLoan(amount, curr, period, out totalCost, out interestRate);

            Console.WriteLine("TotalCost " + totalCost);
            Console.WriteLine("InterestRate " + interestRate);

            Console.WriteLine("8==>");

        }

        public static void getBalance()
        {
            String tmpAccountID;
            if (accountID == null)
            {
                Console.WriteLine("Set your account number");
                tmpAccountID = Console.ReadLine();
            }
            else
            {
                tmpAccountID = accountID;
            }


            Ice.ObjectPrx obj = ic.stringToProxy(tmpAccountID + ":ssl -h " + ip +  " -p 10001");
            AccountPrx account = AccountPrxHelper.checkedCast(obj);

            if (account == null)
                throw new ApplicationException("Invalid account proxy");

            int amount = account.getBalance();
            if (amount == int.MinValue)
            {
                Console.WriteLine("Hola hola jaki certyfikat !");
            }
            else
            {
                Console.WriteLine("Your balance " + amount);
            }

            Console.WriteLine("8==>");
        }

        public static void Main(string[] args)
        {
            int status = 0;

            try
            {
                ic = Ice.Util.initialize(ref args);
                String line = null;
                do
                {
                    Console.WriteLine("8==>");
                    line = Console.ReadLine();
                    if (line == null)
                        break;

                    Console.WriteLine("8==> == " + line);

                    if (line.Equals("c"))
                        createAccount();

                    else if (line.Equals("t"))
                        transfer();

                    else if (line.Equals("d"))
                        delete();

                    else if (line.Equals("l"))
                        loan();

                    else if (line.Equals("b"))
                        getBalance();

                } while (!line.Equals("x"));


                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.ReadKey();
                Console.Error.WriteLine(e);
                status = 1;
            }
            if (ic != null)
            {
                try
                {
                    ic.destroy();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    status = 1;
                }
            }
            Console.ReadKey();
            Environment.Exit(status);
        }

    }
}
