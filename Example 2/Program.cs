using System;
using System.Collections;
using System.Collections.Generic;

namespace Example_2
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is our Observable also known as publisher that notifies about change
            var stockObservable = new Observable<Stock>();

            //observer that monitors microsoft stock
            var microsoftObserver = new MicrosoftStockObserver();
            //here microsoftObserver gets register with stockObservable 
            //as microsoftObserver wants to get notified when there is a 
            //change made inside the subject.
            stockObservable.Register(microsoftObserver);

            //observer that monitors google stock
            var googleObserver = new GoogleStockObserver();
            //here googleObserver gets register with stockObservable 
            //as googleObserver wants to get notified when there is a 
            //change made inside the subject.
            stockObservable.Register(googleObserver);

            //our same old simulator
            var stockSimulator = new StockSimulator();
            //code that updates the subject
            foreach (var stock in stockSimulator)
                stockObservable.Subject = stock; //this will cause for 

            Console.ReadLine();
        }

        public class GoogleStockObserver : IObserver<Stock>
        {
            public void Update(Stock data)
            {
                if (data.Name == "Google" && data.Price > 50)
                    Console.WriteLine($"Google has reached the target price {data.Price}");
            }
        }

        public class MicrosoftStockObserver : IObserver<Stock>
        {
            public void Update(Stock data)
            {
                if (data.Name == "Microsoft")
                    Console.WriteLine($"Microsoft new price is {data.Price}");
            }
        }

        //An generic interface for Observers specifying how they should be updated
        public interface IObserver<T>
        {
            void Update(T data);           
        }

        public class Observable<T>
        {
            private List<IObserver<T>> observers = new List<IObserver<T>>();
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

            public void Register(IObserver<T> observer)
            {
                observers.Add(observer);
            }

            public void Unregister(IObserver<T> observer)
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
