using BusinessLogic.Models;

namespace Presentation.Models
{
    public class TaskView
    {
        private readonly TaskDataTransferObject _taskDTO;

        public TaskView(TaskDataTransferObject taskDTO)
        {
             _taskDTO = taskDTO;
        }

        public override string ToString()
        {
            return $"[ID: {_taskDTO.Id}] {_taskDTO.Name}{(_taskDTO.IsCompleted ? " (COMPLETED) " : "")}\nPriority: {(Priority)_taskDTO.Priority}\nDescription: {_taskDTO.Text}";
        }
    }
}
