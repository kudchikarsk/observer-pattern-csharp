using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockObservable = new Observable<Stock>();

            var microsoftObservable = new MicrosoftStockObservable();
            stockObservable.Register(microsoftObservable);

            var googleObservable = new GoogleStockObservable();
            stockObservable.Register(googleObservable);

            var stockSimulator = new StockSimulator();
            foreach (var stock in stockSimulator)
                stockObservable.Subject = stock;

            Console.ReadLine();
        }

        public class GoogleStockObservable : Observer<Stock>
        {
            public override void Update(Stock data)
            {
                if (data.Name == "Google" && data.Price > 50)
                    Console.WriteLine($"Google has reached the target price {data.Price}");
            }
        }

        public class MicrosoftStockObservable : Observer<Stock>
        {
            public override void Update(Stock data)
            {
                if (data.Name == "Microsoft")
                    Console.WriteLine($"Microsoft new price is {data.Price}");
            }
        }

        public class Observer<T>
        {
            public virtual void Update(T data) { }            
        }

        public class Observable<T>
        {
            private List<Observer<T>> observers = new List<Observer<T>>();
            private T subject;

            public T Subject
            {
                get => subject;
                set 
                {
                    subject = value;
                    Notify();
                }
            }

            public void Register(Observer<T> observer)
            {
                observers.Add(observer);
            }

            public void Unregister(Observer<T> observer)
            {
                observers.Remove(observer);
            }

            public void Notify()
            {
                foreach (var observer in observers)
                {
                    observer.Update(subject);
                }
            }
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
