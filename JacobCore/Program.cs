using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JacobCore
{
    class Program
    {
        public static async Task<bool> ExecuteInParallel<T>(List<T> taskList, Func<T, Task<bool>> function, int throttle = 0)
        {
            bool allSucceeded = true;
            async Task doTasks(List<T> subList)
            {
                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (T item in subList)
                {
                    tasks.Add(function(item));
                }
                await Task.WhenAll(tasks);
                if (!tasks.TrueForAll(x => x.Result == true))
                {
                    allSucceeded = false;
                }
            }
            if (throttle != 0)
            {
                int iteration = 0;
                while (iteration * throttle < taskList.Count)
                {
                    var currentTasks = taskList.Skip(iteration * throttle).Take(throttle).ToList();
                    await doTasks(currentTasks);
                    iteration++;
                }
            }
            else
            {
                await doTasks(taskList);
            }
            return allSucceeded;
        }
    }
}
