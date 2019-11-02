using System;
using System.Collections;
using System.Collections.Generic;

namespace observer_pattern_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            /* The below code is a stock simulator that contains a list of dummy stock data
             * Each stock contains a name and price that we can monitor
             */
            var stockSimulator = new StockSimulator();
            foreach (var stock in stockSimulator)
            {
                //The below code that checks for microsoft stock and display its changed price
                if (stock.Name == "Microsoft")
                    Console.WriteLine($"Microsoft new price is {stock.Price}");

                //The below code looks for google stock and check if its reached the target price 
                // and display the price when it reaches the target price.
                if (stock.Name == "Google" && stock.Price > 50)
                    Console.WriteLine($"Google has reached the target price {stock.Price}");
            }
            Console.ReadLine();
        }

        public class StockSimulator : IEnumerable<Stock>
        {
            Random random;
            string[] names = new string[3] { "Microsoft", "Google", "Apple" };

            public StockSimulator()
            {
                random = new Random(1024);
            }

            public IEnumerator<Stock> GetEnumerator()
            {
                for (int i = 0; i < 20; i++)
                {
                    yield return new Stock(names[random.Next(0, 3)], 
                        random.Next(1, 100));
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class Stock
        {
            public Stock(string name, int value)
            {
                Name = name;
                Price = value;
            }

            public string Name { get; }
            public int Price { get; }
        }
    }
}
