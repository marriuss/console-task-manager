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
            _BL2DAMapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskDataTransferObject, TaskModel>()).CreateMapper();
            _DA2BLMapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskDataObject, TaskDataTransferObject>()).CreateMapper();
        }

        public void AddTask(TaskDataTransferObject taskDTO)
        {
            TaskModel task = TryMap<TaskDataTransferObject, TaskModel>(taskDTO, _BL2DAMapper);

            if (task != null)
            {
                HandleException(() => _tasksRepository.Create(task));
                UpdateCachedData();
            }
        }

        public void RemoveTask(TaskDataTransferObject taskDTO)
        {
            IReadOnlyTaskDataObject taskDO = GetTaskDOById(taskDTO.Id);
            HandleException(() => _tasksRepository.Delete(taskDO));
            UpdateCachedData();
        }

        public void CompleteTask(TaskDataTransferObject taskDTO)
        {
            TaskModel taskModel = TryMap<TaskDataTransferObject, TaskModel>(taskDTO, _BL2DAMapper);

            if (taskModel != null)
            {
                taskModel.IsCompleted = true;
                IReadOnlyTaskDataObject taskDO = GetTaskDOById(taskDTO.Id);
                HandleException(() => _tasksRepository.Update(taskDO, taskModel));
                UpdateCachedData();
            }
        }

        public TaskDataTransferObject GetTaskById(int id)
        {
            IReadOnlyTaskDataObject taskDO = GetTaskDOById(id);
            return TryMap<IReadOnlyTaskDataObject, TaskDataTransferObject>(taskDO, _DA2BLMapper);
        }

        public List<TaskDataTransferObject> GetAllTasks()
        {
            if (_todoList == null)
                UpdateCachedData();

            List<TaskDataTransferObject> tasks = TryMap<List<IReadOnlyTaskDataObject>, List<TaskDataTransferObject>>(_todoList, _DA2BLMapper);

            if (tasks == null)
                tasks = new List<TaskDataTransferObject>();

            return tasks;
        }

        public List<TaskDataTransferObject> SearchTasksByName(string name) => GetAllTasks().FindAll(task => task.Name.Contains(name));

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
