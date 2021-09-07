using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace bluebean
{
    public class TickBaseSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            var task = state as Task;
            System.Console.WriteLine(string.Format("{0} Post thread:{1} taskName:{2}", "TickBaseSynchronizationContext", Thread.CurrentThread.ManagedThreadId, task.AsyncState));
            m_taskQueue.Enqueue(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            System.Console.WriteLine(string.Format("{0} Send thread:{1} cb:{2}", "TickBaseSynchronizationContext", Thread.CurrentThread.ManagedThreadId, 0));
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
}
