using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityData = Verse.UnityData;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// 用于向主线程发送任务
    /// </summary>
    public static class Dispatcher
    {
        private static readonly ConcurrentQueue<Action> _queuedTasks = new ConcurrentQueue<Action>();


        /// <summary>
        /// 在主线程完成任务
        /// </summary>
        /// <param name="task">要完成的任务</param>
        public static Task InvokeAsync(Action task)
        {
            if (UnityData.IsInMainThread)
            {
                task.Invoke();
                return Task.CompletedTask;
            }

            var taskCompletionSource = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    task.Invoke();
                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetException(e);
                }
            }

            _queuedTasks.Enqueue(WrappedAction);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 在主线程完成任务
        /// </summary>
        /// <param name="task">要完成的任务</param>
        public static Task<T> InvokeAsync<T>(Func<T> task)
        {
            if (UnityData.IsInMainThread)
            {
                return Task.FromResult(task.Invoke());
            }

            var taskCompletionSource = new TaskCompletionSource<T>();

            void WrappedAction()
            {
                try
                {
                    taskCompletionSource.TrySetResult(task.Invoke());
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetException(e);
                }
            }

            _queuedTasks.Enqueue(WrappedAction);

            return taskCompletionSource.Task;
        }

        internal static void Update()
        {
            while (!_queuedTasks.IsEmpty)
            {
                if (_queuedTasks.TryDequeue(out var task))
                {
                    task.Invoke();
                }
            }
        }
    }
}
