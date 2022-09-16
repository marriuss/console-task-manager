using System;
using System.Collections.Generic;
using DataAccess.Interfaces;
using DataAccess.Models;
using DataAccess.EF;
using DataAccess.Exceptions;
using System.Data.Entity;
using AutoMapper;

namespace Application.Implementations.TaskRepositories
{
    internal class DatabaseTaskRepository : ITasksRepository
    {
        private readonly TodoListContext _todoListContext;
        private readonly IMapper _BL2DAmapper;

        public DatabaseTaskRepository(string connectionString)
        {
            _todoListContext = new TodoListContext(connectionString);
            _BL2DAmapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskModel, TaskDataObject>()).CreateMapper();
        }

        public void Create(TaskModel task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            TaskDataObject taskDataObject = _BL2DAmapper.Map<TaskDataObject>(task);
            _todoListContext.Tasks.Add(taskDataObject);
        }

        public void Delete(IReadOnlyTaskDataObject taskDO)
        {
            if (taskDO == null) throw new ArgumentNullException(nameof(taskDO));

            TaskDataObject task = _todoListContext.Tasks.Find(taskDO.Id);

            if (task != null)
            {
                _todoListContext.Tasks.Remove(task);
            }
            else throw new TaskDoesNotExistException(taskDO);
        }

        public void Update(IReadOnlyTaskDataObject taskDO, TaskModel taskModel)
        {
            if (taskDO == null) new ArgumentNullException(nameof(taskDO));

            TaskDataObject task = _todoListContext.Tasks.Find(taskDO.Id);

            if (task != null)
            {
                task = _BL2DAmapper.Map(taskModel, task);
                _todoListContext.Entry(task).State = EntityState.Modified;
            }
            else throw new TaskDoesNotExistException(taskDO);
        }

        public List<IReadOnlyTaskDataObject> GetAll()
        {
            return new List<IReadOnlyTaskDataObject>(_todoListContext.Tasks);
        }
    }
}
