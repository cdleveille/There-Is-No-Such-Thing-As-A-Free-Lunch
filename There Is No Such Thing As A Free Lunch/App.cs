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
                    taxAmount = Round(item.Amount * taxRate);
                    taxTotalAmount += taxAmount;
                    extraAmount = Round(prorated ? (item.Amount / subtotal) * tipPlusFees : (1.0 / items.Count) * tipPlusFees);
                    totalAmount = Round(item.Amount + taxAmount + extraAmount);
                    grandTotalAmount += totalAmount;
                    Console.WriteLine(item.Name + ": " + Round(totalAmount).ToString() + " (" + Round(item.Amount).ToString() +
                        " + " + Round(taxAmount) + " tax + " + Round(extraAmount).ToString() + " tip/fees" + ")");
                }

                string proratedString = prorated ? "ON" : "OFF";
                Console.WriteLine();
                Console.WriteLine("Total: " + grandTotalAmount + " (" + subtotal + " subtotal + " + taxTotalAmount + " tax + " + tipPlusFees + " tip/fees)");
                Console.WriteLine("Tip/fees proration is " + proratedString);

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
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
            }
        }

        private static void ReadInputTextFile()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] files = dir.GetFiles("*.txt");
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Please check .txt input file: " + e.Message);
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
