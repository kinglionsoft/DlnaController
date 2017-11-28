using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace TestDemo
{
    public static class ReactiveTester
    {
        public static void Run()
        {
            var observable = Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                //Observable.Timer(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2))
                //    .Subscribe(o => observer.OnNext((int)o));
                return Task.CompletedTask;
            });

            //Observable.Interval(TimeSpan.FromSeconds(2))


            observable = Observable.Range(1, 10);

            observable = Observable.Start(() => 0);

            observable.Subscribe(o =>
             {
                 Console.WriteLine(o);
             });
        }


    }
}