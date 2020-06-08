using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TickBaseSynchronizationContext
{
    /// <summary>
    /// 基于Tick的SynchronizationContext
    /// </summary>
    public class TickBaseSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            System.Console.WriteLine(string.Format("Post thread:{0} cb:{1}", Thread.CurrentThread.ManagedThreadId,0));
            m_taskQueue.Enqueue(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            System.Console.WriteLine(string.Format("Send thread:{0},cb:{1}", Thread.CurrentThread.ManagedThreadId,0));
            m_taskQueue.Enqueue(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        /// <summary>
        /// 驱动task调度
        /// </summary>
        /// <returns></returns>
        public bool Tick()
        {
            KeyValuePair<SendOrPostCallback, object> item;
            if (m_taskQueue.TryDequeue(out item))
            {
                try
                {
                    item.Key(item.Value);
                }
                catch (Exception e)
                {
                    if (EventOnException != null)
                    {
                        EventOnException(e);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通知使用者调度的task抛出异常
        /// </summary>
        public event Action<Exception> EventOnException;

        /// <summary>
        /// 调度队列
        /// </summary>
        private ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>> m_taskQueue = new ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>>();
    }

    class LogicThread
    {
        private Thread m_thread;
        private CancellationTokenSource m_cancelSource = null;
        private CancellationToken m_cancelToken;
        private TaskScheduler m_taskScheduler;
        protected TickBaseSynchronizationContext m_syncCtx;

        public void Start()
        {
            // 首先设置SynchronizationContext和TaskScheduler
            m_syncCtx = new TickBaseSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(m_syncCtx);
            m_taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // 初始化cancel现场
            m_cancelSource = new CancellationTokenSource();
            m_cancelToken = m_cancelSource.Token;

            Task.Factory.StartNew(ManagedThreadProc,"managedTask1", m_cancelToken, TaskCreationOptions.LongRunning, m_taskScheduler);
            Task.Factory.StartNew(ManagedThreadProc, "managedTask2", m_cancelToken, TaskCreationOptions.LongRunning, m_taskScheduler);

            m_thread = new Thread(MainThreadProc);
            m_thread.Start();
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="obj"></param>
        protected async void ManagedThreadProc(Object state)
        {
            while (!m_cancelToken.IsCancellationRequested)
            {
                System.Console.WriteLine(string.Format("{0} thread:{1}", state, Thread.CurrentThread.ManagedThreadId));
                Thread.Sleep(10);
                await Task.Yield();
            }
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        /// <param name="obj"></param>
        protected void MainThreadProc(object obj)
        {
            while (!m_cancelToken.IsCancellationRequested)
            {
                m_syncCtx.Tick(); 
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            m_cancelSource.Cancel();
            // 调用dispose，释放资源
            m_cancelSource.Dispose();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            LogicThread logicThread = new LogicThread();
            logicThread.Start();
            Thread.Sleep(3000);
            logicThread.Stop();
        }
    }
}
