using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using DataAccess.Models;
using DataAccess.Interfaces;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;

namespace Application.Implementations.TaskManagers
{
    public class TaskManager : ITaskManager
    {
        private List<IReadOnlyTaskDataObject> _todoList;
        private readonly ITasksRepository _tasksRepository;
        private readonly IMapper _BL2DAMapper;
        private readonly IMapper _DA2BLMapper;

        public event EventHandler<ErrorEventArgs> ErrorOccured;

        public TaskManager(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
            _BL2DAMapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskOTD, TaskModel>()).CreateMapper();
            _DA2BLMapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskDataObject, TaskOTD>()).CreateMapper();
        }

        public void AddTask(TaskOTD taskOTD)
        {
            TaskModel task = TryMap<TaskOTD, TaskModel>(taskOTD, _BL2DAMapper);

            if (task != null)
            {
                HandleException(() => _tasksRepository.Create(task));
                UpdateCachedData();
            }
        }

        public void RemoveTask(TaskOTD taskOTD)
        {
            IReadOnlyTaskDataObject taskDO = GetTaskDOById(taskOTD.Id);
            HandleException(() => _tasksRepository.Delete(taskDO));
            UpdateCachedData();
        }

        public void CompleteTask(TaskOTD taskOTD)
        {
            TaskModel taskModel = TryMap<TaskOTD, TaskModel>(taskOTD, _BL2DAMapper);

            if (taskModel != null)
            {
                taskModel.IsCompleted = true;
                IReadOnlyTaskDataObject taskDO = GetTaskDOById(taskOTD.Id);
                HandleException(() => _tasksRepository.Update(taskDO, taskModel));
                UpdateCachedData();
            }
        }

        public TaskOTD GetTaskById(int id)
        {
            IReadOnlyTaskDataObject taskDO = GetTaskDOById(id);
            return TryMap<IReadOnlyTaskDataObject, TaskOTD>(taskDO, _DA2BLMapper);
        }

        public List<TaskOTD> GetAllTasks()
        {
            if (_todoList == null) UpdateCachedData();

            List<TaskOTD> tasks = TryMap<List<IReadOnlyTaskDataObject>, List<TaskOTD>>(_todoList, _DA2BLMapper);

            if (tasks == null) tasks = new List<TaskOTD>();

            return tasks;
        }

        public List<TaskOTD> SearchTasksByName(string name) => GetAllTasks().FindAll(task => task.Name.Contains(name));

        public void SortTasksByPriority() => _todoList = _todoList.OrderByDescending(taskDO => taskDO.Priority).ToList();

        private void UpdateCachedData() => HandleException(() => _todoList = _tasksRepository.GetAll());

        private IReadOnlyTaskDataObject GetTaskDOById(int id) => _todoList.Find(taskDO => taskDO.Id == id);

        private T TryMap<K, T>(K model, IMapper mapper)
        {
            T result = default;
            HandleException(() => result = mapper.Map<T>(model));
            return result;
        }

        private void HandleException(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex.Source, new ErrorEventArgs(ex));
            }
        }
    }
}
