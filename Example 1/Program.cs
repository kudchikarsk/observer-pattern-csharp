using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_1
{
    class Program
    {
        static void Main(string[] args)
        {

            var stockSimulator = new StockSimulator();
            foreach (var stock in stockSimulator)
            {
                if (stock.Name == "Microsoft")
                    Console.WriteLine($"Microsoft new price is {stock.Price}");

                if (stock.Name == "Google" && stock.Price > 50)
                    Console.WriteLine($"Google has reached the target price {stock.Price}");

                /* Now, if look at the program we can say that we have two differenct activity.
                 * One activity is checking for the microsoft price and the other is checking for the google price.
                 * Although, they both are independent of each other they are occuring in the same program.
                 * 
                 * If for some reason we want to monitor a new stock we have to update the main program.
                 */

                if (stock.Name == "Apple")
                    Console.WriteLine($"Apple new price is {stock.Price}");

                /* Now, The observer pattern what allow us to do is separate out the monitoring 
                 * aspect of the stock data from the action of reading the stock data from the stock simulator.
                 */
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
