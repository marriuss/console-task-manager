using System;
using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface ITasksRepository
    {
        void Create(TaskModel task);
        void Delete(IReadOnlyTaskDataObject taskDO);
        void Update(IReadOnlyTaskDataObject taskDO, TaskModel taskModel);
        List<IReadOnlyTaskDataObject> GetAll();
    }
}