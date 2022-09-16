using BusinessLogic.Models;

namespace Presentation.Models
{
    public class TaskView
    {
        private readonly TaskOTD _taskOTD;

        public TaskView(TaskOTD taskOTD)
        {
             _taskOTD = taskOTD;
        }

        public override string ToString()
        {
            return $"[ID: {_taskOTD.Id}] {_taskOTD.Name}{(_taskOTD.IsCompleted ? " (COMPLETED) " : "")}\nPriority: {(Priority)_taskOTD.Priority}\nDescription: {_taskOTD.Text}";
        }
    }
}
