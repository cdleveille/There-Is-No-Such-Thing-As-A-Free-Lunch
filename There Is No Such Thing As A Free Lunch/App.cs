using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace There_Is_No_Such_Thing_As_A_Free_Lunch
{
    class App
    {
        private static double tipPlusFees;
        private static bool prorated;
        private static double taxRate;
        private static List<OrderItem> items = new List<OrderItem>();

        public static void Main(string[] args)
        {
            double subtotal = 0, grandTotalAmount = 0, taxTotalAmount = 0;
            double participantTaxAmount, participantExtraAmount, participantTotalAmount;

            try
            {
                ReadConfigFile();
                ReadInputTextFile();

                foreach (OrderItem item in items)
                {
                    subtotal += item.Amount;
                }

                foreach (OrderItem item in items)
                {
                    participantTaxAmount = item.Amount * taxRate * 0.01;
                    participantExtraAmount = prorated ? item.Amount * tipPlusFees / subtotal : tipPlusFees / items.Count;
                    participantTotalAmount = item.Amount + participantTaxAmount + participantExtraAmount;
                    taxTotalAmount += participantTaxAmount;
                    grandTotalAmount += participantTotalAmount;
                    Console.WriteLine(item.Name + ": " + Round(participantTotalAmount) + " (" + Round(item.Amount) +
                        " + " + Round(participantTaxAmount) + " tax + " + Round(participantExtraAmount) + " tip/fees" + ")");
                }

                Console.WriteLine();
                Console.WriteLine("Total: " + Round(grandTotalAmount) + " (" + Round(subtotal) + " subtotal + " + Round(taxTotalAmount) + " tax + " + tipPlusFees + " tip/fees)");
                Console.WriteLine("Tax rate: " + Round(taxRate) + "%");
                Console.WriteLine("Tip/fees proration is " + (prorated ? "ON" : "OFF"));
                Console.ReadLine();
            }
            catch (Exception e)
            {
                AbendWithMessage("Error! " + e.Message);
            }
        }

        private static void ReadConfigFile()
        {
            try
            {
                prorated = Convert.ToBoolean(ConfigurationSettings.AppSettings["prorated"]);
                taxRate = Convert.ToDouble(ConfigurationSettings.AppSettings["taxRate"]);
            }
            catch (Exception e)
            {
                AbendWithMessage("Error! Please check config file: " + e.Message);
            }
        }

        private static void ReadInputTextFile()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] files = dir.GetFiles("*.txt");

                if (files.Length < 1)
                {
                    AbendWithMessage("Error! Cannot find input .txt file in the application directory.");
                }
                else if (files.Length > 1)
                {
                    AbendWithMessage("Error! There is more than one .txt file in the application directory.");
                }

                StreamReader file = new StreamReader(files[0].FullName);

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(' ');
                    if (splitLine.Length > 1)
                    {
                        items.Add(new OrderItem(splitLine[0], Convert.ToDouble(splitLine[1])));
                    }
                    else
                    {
                        tipPlusFees = Convert.ToDouble(splitLine[0]);
                    }
                }

                file.Close();

                if (items.Count == 0)
                {
                    throw new Exception("No order items specified.");
                }
            }
            catch (Exception e)
            {
                AbendWithMessage("Error! Please check .txt input file: " + e.Message);
            }
        }

        private static double Round(double amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        private static void AbendWithMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(1);
        }
    }

    class OrderItem
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public OrderItem(string name, double amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
