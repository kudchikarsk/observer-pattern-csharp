using System;
using System.Collections;
using System.Collections.Generic;

namespace Example_3
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockObservable = new Observable<Stock>();

            var microsoftObserver = new MicrosoftStockObserver();
            stockObservable.Subscribe(microsoftObserver);

            var googleObserver = new GoogleStockObserver();
            stockObservable.Subscribe(googleObserver);

            var stockSimulator = new StockSimulator();
            foreach (var stock in stockSimulator)
                stockObservable.Subject = stock;

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

            public Unsubscriber<T> Subscribe(IObserver<T> observer)
            {
                if(!observers.Contains(observer))
                    observers.Add(observer);
                return new Unsubscriber<T>(observers, observer);
            }

            public void Notify()
            {
                foreach (var observer in observers)
                {
                    observer.Update(subject);
                }
            }
        }

        public class Unsubscriber<T> : IDisposable
        {
            private List<IObserver<T>> observers;
            private IObserver<T> observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                observers.Remove(observer);
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
