using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bluebean
{
    class Program
    {
        static SynchronizationContext context;

        static void TestThread()
        {
            var thrd = new Thread(Start);
            thrd.Start();
        }

        static void Start()
        {
            Console.WriteLine("子线程id：" + Thread.CurrentThread.ManagedThreadId);
            context.Send(EventMethod, "子线程Send");
            context.Post(EventMethod, "子线程Post");
            Console.WriteLine("子线程结束");
        }

        static void EventMethod(object arg)
        {
            Console.WriteLine("CallBack::当前线程id：" + Thread.CurrentThread.ManagedThreadId + "     arg:" + (string)arg);
        }

        static void Main(string[] args)
        {
            /*
            context = new SynchronizationContext();
            Console.WriteLine("主线程id：" + Thread.CurrentThread.ManagedThreadId);
            TestThread();
            Thread.Sleep(6000);
            Console.WriteLine("主线程执行");
            context.Send(EventMethod, "Send");
            context.Post(EventMethod, "Post");
            Console.WriteLine("主线程结束");
            */

            ///*
            LogicThread logicThread = new LogicThread();
            logicThread.Start();
            Thread.Sleep(10000);
            logicThread.Stop();
            //*/
        }
    }
}
