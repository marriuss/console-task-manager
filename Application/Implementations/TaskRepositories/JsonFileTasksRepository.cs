using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using DataAccess.Interfaces;
using DataAccess.Models;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Implementations.TaskRepositories
{
    public class JsonFileTasksRepository : ITasksRepository
    {
        private const string FileName = "results.json";
        private readonly ILogger _logger;
        private readonly IMapper _BL2DAmapper;
        private string _filePath;
        private List<TaskDataObject> _taskDOs;
        private int _lastId;

        private string DefaultDirectory => Directory.GetCurrentDirectory();

        public JsonFileTasksRepository(ILogger logger)
        {
            _logger = logger;
            string fileDirectory = ConfigurationManager.AppSettings.Get("fileDirectory");
            _filePath = GetFilePath(fileDirectory);
            _taskDOs = new List<TaskDataObject>();
            _BL2DAmapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskModel, TaskDataObject>()).CreateMapper();
        }

        public void Create(TaskModel task)
        {
            if (task == null)
            {
                _logger.LogError("Null task cannot be added.");
                return;
            }

            int id = GetLastId();
            TaskDataObject taskDataObject = _BL2DAmapper.Map<TaskDataObject>(task);
            taskDataObject.Id = id;
            _taskDOs.Add(taskDataObject);
            SaveData();
        }

        public void Update(IReadOnlyTaskDataObject taskDO, TaskModel taskModel)
        {
            if (taskDO == null)
            {
                _logger.LogError("Null task cannot be modified.");
                return;
            }

            if (taskModel == null)
            {
                _logger.LogError("Null task model argument.");
                return;
            }

            if (_taskDOs.Contains(taskDO))
            {
                TaskDataObject task = _taskDOs.Find(item => item.Equals(taskDO));
                task = _BL2DAmapper.Map(taskModel, task);
                SaveData();
            }
            else _logger.LogError("Requiested task doesn't exist.");
        }

        public void Delete(IReadOnlyTaskDataObject taskDO)
        {
            if (taskDO == null)
            {
                _logger.LogError("Null task cannot be deleted.");
                return;
            }

            if (_taskDOs.Contains(taskDO))
            {
                TaskDataObject task = _taskDOs.Find(item => item.Equals(taskDO));
                _taskDOs.Remove(task);
                SaveData();
            }
            else _logger.LogError("Requiested task doesn't exist.");
        }

        public List<IReadOnlyTaskDataObject> GetAll()
        {
            if (_taskDOs.Count == 0)
                LoadData();

            return new List<IReadOnlyTaskDataObject>(_taskDOs);
        }

        private void SaveData()
        {
            var sortedTasks = _taskDOs.OrderBy(taskDO => taskDO.Id);

            using (StreamWriter sw = File.CreateText(_filePath))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(sw, sortedTasks);
            }
        }

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string fileText = File.ReadAllText(_filePath);

                    if (!string.IsNullOrEmpty(fileText))
                        _taskDOs = JsonConvert.DeserializeObject<List<TaskDataObject>>(fileText);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured while loading json data: {ex.Message}.");
                }
            }
            else
            {
                _logger.LogInformation("File doesn't exist. New file creation initialized.");

                try
                {
                    File.Create(_filePath);
                }
                catch (IOException exception)
                {
                    _logger.LogCritical($"File cannot be created: {exception.Message}.");

                    if (_filePath != DefaultDirectory)
                    {
                        _filePath = GetFilePath(DefaultDirectory);
                        _logger.LogInformation($"File path set to default ({_filePath}).");
                        LoadData();
                    }
                }
            }

            _lastId = _taskDOs.Count == 0 ? 0 : _taskDOs.OrderByDescending(taskDO => taskDO.Id).First().Id;
        }

        private int GetLastId()
        {
            bool doesIdExist;
            int id;

            do
            {
                id = ++_lastId;
                doesIdExist = _taskDOs.Exists(taskDO => taskDO.Id == id);
            }
            while (doesIdExist);

            return id;
        }

        private string GetFilePath(string fileDirectory)
        {
            bool isValid;

            try
            {
                fileDirectory = Path.GetFullPath(fileDirectory);
                isValid = !string.IsNullOrEmpty(fileDirectory);
                isValid &= Directory.Exists(fileDirectory);
            }
            catch (ArgumentException)
            {
                isValid = false;
            }

            if (!isValid)
            {
                fileDirectory = DefaultDirectory;
                _logger.LogWarning($"Invalid file directory. File directory set to default ({DefaultDirectory}).");
            }

            return Path.Combine(fileDirectory, FileName);
        }
    }
}
