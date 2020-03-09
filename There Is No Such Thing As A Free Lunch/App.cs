using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace There_Is_No_Such_Thing_As_A_Free_Lunch
{
    class App
    {
        static double tipPlusFees;
        static double taxRate;
        static bool prorated;
        static List<OrderItem> items = new List<OrderItem>();

        public static void Main(string[] args)
        {
            double subtotal = 0, grandTotalAmount = 0, taxTotalAmount = 0;
            double taxAmount, extraAmount, totalAmount;

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
                    taxAmount = item.Amount * taxRate;
                    taxTotalAmount += taxAmount;
                    extraAmount = prorated ? (item.Amount / subtotal) * tipPlusFees : (1.0 / items.Count) * tipPlusFees;
                    totalAmount = item.Amount + taxAmount + extraAmount;
                    grandTotalAmount += totalAmount;
                    Console.WriteLine(item.Name + ": " + Round(totalAmount).ToString() + " (" + Round(item.Amount).ToString() +
                        " + " + Round(taxAmount) + " tax + " + Round(extraAmount).ToString() + " tip/fees" + ")");
                }

                string proratedString = prorated ? "ON" : "OFF";
                Console.WriteLine();
                Console.WriteLine("Total: " + Round(grandTotalAmount) + " (" + Round(subtotal) + " subtotal + " + Round(taxTotalAmount) + " tax + " + Round(tipPlusFees) + " tip/fees)");
                Console.WriteLine("Tax rate: " + Round(taxRate * 100).ToString() + "%");
                Console.WriteLine("Tip/fees proration is " + proratedString);

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private static void ReadConfigFile()
        {
            try
            {
                taxRate = Convert.ToDouble(ConfigurationSettings.AppSettings["taxRate"]) * 0.01;
                prorated = Convert.ToBoolean(ConfigurationSettings.AppSettings["prorated"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Please check config file: " + e.Message);
                Console.ReadLine();
                Environment.Exit(1);
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
                    Console.WriteLine("Error! Cannot find input .txt file in the current directory.");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                else if (files.Length > 1)
                {
                    Console.WriteLine("Error! There is more than one .txt file in the current directory.");
                    Console.ReadLine();
                    Environment.Exit(1);
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

                if (items.Count < 2)
                {
                    throw new Exception("Not enough order items specified.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Please check .txt input file: " + e.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private static double Round(double amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
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
