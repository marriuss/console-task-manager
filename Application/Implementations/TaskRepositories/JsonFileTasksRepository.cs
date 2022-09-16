﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Exceptions;
using DataAccess.Interfaces;
using DataAccess.Models;
using Newtonsoft.Json;
using AutoMapper;

namespace Application.Implementations.TaskRepositories
{
    public class JsonFileTasksRepository : ITasksRepository
    {
        private readonly string _filePath;
        private readonly IMapper _BL2DAmapper;
        private List<TaskDataObject> _taskDOs;
        private int _lastId;

        public JsonFileTasksRepository(string filePath)
        {
            _taskDOs = new List<TaskDataObject>();
            _BL2DAmapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskModel, TaskDataObject>()).CreateMapper();
            _filePath = filePath;
        }

        public void Create(TaskModel task)
        {
            int id = GetLastId();
            TaskDataObject taskDataObject = _BL2DAmapper.Map<TaskDataObject>(task);
            taskDataObject.Id = id;
            _taskDOs.Add(taskDataObject);
            SaveData();
        }

        public void Update(IReadOnlyTaskDataObject taskDO, TaskModel taskModel)
        {
            if (_taskDOs.Contains(taskDO))
            {
                TaskDataObject task = _taskDOs.Find(item => item.Equals(taskDO));
                task = _BL2DAmapper.Map(taskModel, task);
                SaveData();
            }
            else throw new TaskDoesNotExistException(taskDO);
        }

        public void Delete(IReadOnlyTaskDataObject taskDO)
        {
            if (_taskDOs.Contains(taskDO))
            {
                TaskDataObject task = _taskDOs.Find(item => item.Equals(taskDO));
                _taskDOs.Remove(task);
                SaveData();
            }
            else throw new TaskDoesNotExistException(taskDO);
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
                    if (!string.IsNullOrEmpty(fileText)) _taskDOs = JsonConvert.DeserializeObject<List<TaskDataObject>>(fileText);
                }
                catch (Exception)
                {
                    throw new FileDataLoadException(_filePath);
                }
            }
            else File.Create(_filePath);

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
    }
}