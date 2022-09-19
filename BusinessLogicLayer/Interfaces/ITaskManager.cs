using System;
using System.Collections.Generic;
using BusinessLogic.Models;

namespace BusinessLogic.Interfaces
{
    public class ErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; private set; }

        public ErrorEventArgs(string message)
        {
            ErrorMessage = message;
        }

        public ErrorEventArgs(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public interface ITaskManager
    {
        event EventHandler<ErrorEventArgs> ErrorOccured;
        void AddTask(TaskDataTransferObject taskDTO);
        void RemoveTask(TaskDataTransferObject taskDTO);
        void CompleteTask(TaskDataTransferObject taskDTO);
        TaskDataTransferObject GetTaskById(int id);
        List<TaskDataTransferObject> SearchTasksByName(string name);
        void SortTasksByPriority();
        List<TaskDataTransferObject> GetAllTasks();
    }
}
