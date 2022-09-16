using System;
using DataAccess.Models;

namespace DataAccess.Exceptions
{
    public class TaskDoesNotExistException : Exception
    {
        public TaskDoesNotExistException(IReadOnlyTaskDataObject taskDO) : base($"Task with id {taskDO.Id} does not exist.") { }
    }
}
