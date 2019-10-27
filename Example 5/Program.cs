using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Example_5
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockObservable = new Observable<Stock>();

            var microsoftObserver = new MicrosoftStockObserver();
            microsoftObserver.Subscribe(stockObservable);

            var googleObserver = new GoogleStockObserver();
            googleObserver.Subscribe(stockObservable);

            stockObservable.Subject = new Stock("Microsoft", 10);
            microsoftObserver.Unsubscribe();
            stockObservable.Subject = new Stock("Microsoft", 20);
            stockObservable.Subject = new Stock("Microsoft", 30);
            microsoftObserver.Subscribe(stockObservable);
            stockObservable.Subject = new Stock("Microsoft", 40);
            stockObservable.Subject = new Stock("Google", 60);
            googleObserver.Unsubscribe();
            stockObservable.Subject = new Stock("Google", 70);
            stockObservable.Subject = new Stock("Google", 80);
            googleObserver.Subscribe(stockObservable);
            stockObservable.Subject = new Stock("Google", 90);
            stockObservable.Error(new StockNotFoundException("Some error occured!"));
            stockObservable.Completed();
            Console.ReadLine();
        }

        public class GoogleStockObserver : Observer<Stock>
        {
            public override void OnCompleted()
            {
                Console.WriteLine("Stock transmission is completed!");
            }

            public override void OnError(Exception error)
            {
                Console.WriteLine(error.Message);
            }

            public override void OnNext(Stock value)
            {
                if (value.Name == "Google" && value.Price > 50)
                    Console.WriteLine($"Google has reached the target price {value.Price}");
            }
        }

        public class MicrosoftStockObserver : Observer<Stock>
        {
            public override void OnCompleted()
            {
                //not implemented
            }

            public override void OnError(Exception error)
            {
                //not implemented
            }

            public override void OnNext(Stock value)
            {
                if (value.Name == "Microsoft")
                    Console.WriteLine($"Microsoft new price is {value.Price}");
            }
        }

        public abstract class Observer<T> : IObserver<T>
        {
            private Unsubscriber<T> cancellation;

            public void Subscribe(Observable<T> provider)
            {
                cancellation = provider.Subscribe(this);
            }

            public virtual void Unsubscribe()
            {
                cancellation.Dispose();
            }

            public abstract void OnNext(T value);

            public abstract void OnError(Exception error);

            public abstract void OnCompleted();
        }

        public class Observable<T> : IObservable<T>
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

            public void Notify()
            {
                foreach (var observer in observers)
                {
                    observer.OnNext(subject);
                }
            }

            public Unsubscriber<T> Subscribe(Observer<T> observer)
            {
                if (!observers.Contains(observer))
                    observers.Add(observer);
                return new Unsubscriber<T>(observers, observer);
            }

            IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
            {
                return Subscribe(observer as Observer<T>);
            }

            public void Completed()
            {
                foreach (var observer in observers)
                    observer.OnCompleted();
            }

            public void Error(Exception e)
            {
                foreach (var observer in observers)
                    observer.OnError(e);
            }
        }

        public class Unsubscriber<T> : IDisposable
        {
            private List<Observer<T>> observers;
            private Observer<T> observer;

            public Unsubscriber(List<Observer<T>> observers, Observer<T> observer)
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

        [Serializable]
        internal class StockNotFoundException : Exception
        {
            public StockNotFoundException()
            {
            }

            public StockNotFoundException(string message) : base(message)
            {
            }

            public StockNotFoundException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected StockNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
