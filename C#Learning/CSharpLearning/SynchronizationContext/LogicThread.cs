using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bluebean
{
    public class LogicThread
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
            System.Console.WriteLine(string.Format("LogicThread:Start thread:{0}", Thread.CurrentThread.ManagedThreadId));

            Task.Factory.StartNew(ManagedThreadProc, "managedTask1", m_cancelToken, TaskCreationOptions.LongRunning, m_taskScheduler);
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
                System.Console.WriteLine(string.Format("ManagedThreadProc {0} thread:{1}", state, Thread.CurrentThread.ManagedThreadId));
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
                System.Console.WriteLine(string.Format("MainThreadProc Tick threadId: {0}",Thread.CurrentThread.ManagedThreadId));
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
}
