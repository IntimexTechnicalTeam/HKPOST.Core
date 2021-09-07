using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks
{
    public static class TaskExtension
    {
        public static TResult GetResult<TResult>(this Task<TResult> task)
        {
            return task.GetAwaiter().GetResult();
        }
        public static void GetResult(this Task task)
        {
            task.GetAwaiter().GetResult();
        }

        public static TResult GetResult<TResult>(this ValueTask<TResult> task)
        {
            return task.GetAwaiter().GetResult();
        }
    }
}
