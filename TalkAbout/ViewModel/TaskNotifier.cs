using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkAbout.ViewModel
{
    public class TaskNotifier
    {
        private ITaskCompleteNotifier _receiver;
        private Task _task;
        private string _identifier;

        public TaskNotifier(Task task, ITaskCompleteNotifier receiver, string identifier)
        {
            _task = task;
            _receiver = receiver;
            _identifier = identifier;
            if (!_task.IsCompleted)
            {
                var _ = WatchTaskAsync(_task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            _receiver.TaskComplete(task, _identifier);
        }
    }
}
