using System;
using System.Collections.Concurrent;
using System.Threading;
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
        /// 在主线程执行指定的任务
        /// </summary>
        /// <param name="task">要执行的任务</param>
        public static void Invoke(Action task)
        {
            if (UnityData.IsInMainThread)
            {
                task.Invoke();
                return;
            }

            Exception exception = null;

            using (var mre = new ManualResetEvent(false))
            {
                void WrappedAction()
                {
                    try
                    {
                        task.Invoke();
                    }
                    catch (Exception e)
                    {
                        exception = new TaskCanceledException($"Failed to finish a dispatcher task.", e);
                    }
                    finally
                    {
                        mre.Set();
                    }
                }

                _queuedTasks.Enqueue(WrappedAction);
                mre.WaitOne();
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 在主线程执行指定的任务
        /// </summary>
        /// <typeparam name="T">任务返回的结果类型</typeparam>
        /// <param name="task">要执行的任务</param>
        /// <returns>任务的结果。</returns>
        public static T Invoke<T>(Func<T> task)
        {
            if (UnityData.IsInMainThread)
            {
                return task.Invoke();
            }

            Exception exception = null;
            T result = default;

            using (var mre = new ManualResetEvent(false))
            {
                void WrappedAction()
                {
                    try
                    {
                        result = task.Invoke();
                    }
                    catch (Exception e)
                    {
                        exception = new TaskCanceledException($"Failed to finish a dispatcher task.", e);
                    }
                    finally
                    {
                        mre.Set();
                    }
                }

                _queuedTasks.Enqueue(WrappedAction);
                mre.WaitOne();
            }

            if (exception != null)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// 在主线程执行指定的任务
        /// </summary>
        /// <param name="task">要执行的任务</param>
        /// <returns>表示异步操作的 <see cref="Task"/> 对象。</returns>
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
        /// 在主线程执行指定的任务并返回结果
        /// </summary>
        /// <typeparam name="T">任务返回的结果类型</typeparam>
        /// <param name="task">要执行的任务</param>
        /// <returns>表示异步操作的 <see cref="Task"/> 对象，包含任务的结果。</returns>
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
            if (_queuedTasks.IsEmpty)
            {
                return;
            }

            while (_queuedTasks.TryDequeue(out var task))
            {
                task.Invoke();
            }
        }
    }
}
