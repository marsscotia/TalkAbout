using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TalkAbout.ViewModel
{
    public interface ITaskCompleteNotifier
    {
        void TaskComplete(Task task, string identifier);
    }
}
