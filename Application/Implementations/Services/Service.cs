using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Presentation.Models;
using Presentation.Interfaces;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using AutoMapper;

namespace Application.Implementations.Services
{
    public class Service : IService
    {
        private readonly ITaskManager _taskManager;
        private readonly IMapper _Presentation2BLMapper;
        private readonly IUserInterface _userInterface;

        public Service(IUserInterface userInterface, ITaskManager taskManager)
        {
            _userInterface = userInterface;
            _taskManager = taskManager;
            _Presentation2BLMapper = new MapperConfiguration(cfg => cfg.CreateMap<UserTask, TaskOTD>()).CreateMapper();
        }

        public void Start()
        {
            try
            {
                _userInterface.ChooseAction(
                    new Dictionary<string, Action>(){
                   { "AddNewTask", AddNewTask },
                   { "SelectTask", SelectTask },
                   { "SearchByName", SearchByName },
                   { "ShowAllTasks", ShowAllTasks },
                   { "SortTasksByPriority", SortTasks },
                 }
                );
            }
            catch (Exception ex)
            {
                OnErrorOccured(ex.Source, new ErrorEventArgs(ex.Message));
            }
        }

        private void SelectTask()
        {
            int id = _userInterface.AskIntInput("Task ID: ");
            TaskOTD taskOTD = null;
            InteractTaskManager(() => taskOTD = _taskManager.GetTaskById(id));

            if (taskOTD != null)
            {
                _userInterface.ChooseAction(
                    new Dictionary<string, Action>()
                    {
                       { "Remove", () => RemoveTask(taskOTD) },
                       { "Complete", () => MarkTaskAsCompleted(taskOTD) },
                    }
                );
            }
            else _userInterface.ShowMessage("Such task doesn't exist.");
        }

        private void AddNewTask()
        {
            UserTask userTask = AskUserTaskInput();
            TaskOTD taskOTD = _Presentation2BLMapper.Map<TaskOTD>(userTask);
            InteractTaskManager(() => _taskManager.AddTask(taskOTD));
        }

        private void RemoveTask(TaskOTD taskOTD) => InteractTaskManager(() => _taskManager.RemoveTask(taskOTD));

        private void MarkTaskAsCompleted(TaskOTD taskView) => InteractTaskManager(() => _taskManager.CompleteTask(taskView));

        private void SearchByName()
        {
            string name = AskTaskName();
            List<TaskOTD> tasksOTDs = new List<TaskOTD>();
            InteractTaskManager(() => tasksOTDs = _taskManager.SearchTasksByName(name));
            ShowTaskList(tasksOTDs);
        }

        private void ShowAllTasks()
        {
            List<TaskOTD> tasks = new List<TaskOTD>();
            InteractTaskManager(() => tasks = _taskManager.GetAllTasks());
            ShowTaskList(tasks);
        }

        private void SortTasks()
        {
            InteractTaskManager(() => _taskManager.SortTasksByPriority());
            ShowAllTasks();
        }

        private void InteractTaskManager(Action action)
        {
            _taskManager.ErrorOccured += OnErrorOccured;
            action?.Invoke();
            _taskManager.ErrorOccured -= OnErrorOccured;
        }

        private void ShowTaskList(List<TaskOTD> tasks)
        {
            string message = "No results found.";

            if (tasks.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder("Results:\n");

                foreach (TaskOTD task in tasks)
                    stringBuilder.AppendLine("\n" + new TaskView(task).ToString());

                message = stringBuilder.ToString();
            }

            _userInterface.ShowMessage(message);
        }

        private void OnErrorOccured(object sender, ErrorEventArgs e) => _userInterface.ShowMessage($"[Error occured] {sender}: {e.ErrorMessage}");

        private string AskTaskName() => _userInterface.AskStringInput("Name: ");

        private UserTask AskUserTaskInput()
        {
            string name = AskTaskName();
            string text = _userInterface.AskStringInput("Text: ");
            var priorityValues = from value in (Priority[])Enum.GetValues(typeof(Priority)) select $"{(int)value} - {value}";
            Priority priority = _userInterface.AskEnumValueInput<Priority>($"Priority options: {string.Join(", ", priorityValues)}. Choose task priority: ");
            return new UserTask(name, priority, text);
        }
    }
}
