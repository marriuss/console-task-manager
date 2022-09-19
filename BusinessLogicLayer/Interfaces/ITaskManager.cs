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
        void AddTask(TaskOTD taskOTD);
        void RemoveTask(TaskOTD taskOTD);
        void CompleteTask(TaskOTD taskOTD);
        TaskOTD GetTaskById(int id);
        List<TaskOTD> SearchTasksByName(string name);
        void SortTasksByPriority();
        List<TaskOTD> GetAllTasks();
    }
}
