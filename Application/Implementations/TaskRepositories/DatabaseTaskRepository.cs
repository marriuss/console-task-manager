using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using DataAccess.Interfaces;
using DataAccess.Models;
using System.Data.SqlClient;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Implementations.TaskRepositories
{
    internal class DatabaseTaskRepository : ITasksRepository
    {
        private readonly ILogger _logger;
        private readonly IMapper _BL2DAmapper;
        private readonly string _connectionString;

        public DatabaseTaskRepository(ILogger logger)
        {
            _logger = logger;
            _connectionString = "";
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["default"];
            
            try
            {
                if (connectionStringSettings == null)
                    throw new NullReferenceException();

                _connectionString = connectionStringSettings.ConnectionString;

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                }
            }
            catch(SqlException sqlException)
            {
                _logger.LogCritical($"Database connection error: {sqlException.Message}.");
                return;
            }
            catch(NullReferenceException)
            {
                _logger.LogCritical($"Error: connection string value not found.");
                return;
            }

            _BL2DAmapper = new MapperConfiguration(cfg => cfg.CreateMap<TaskModel, TaskDataObject>()).CreateMapper();
            _logger = logger;
            InitializeDatabase();
        }

        public void Create(TaskModel task)
        {
            if (task == null)
            {
                _logger.LogError("Null task cannot be added.");
                return;
            }

            TaskDataObject taskDataObject = _BL2DAmapper.Map<TaskDataObject>(task);

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@name", taskDataObject.Name),
                new SqlParameter("@priority", taskDataObject.Priority),
                new SqlParameter("@text", taskDataObject.Text),
                new SqlParameter("@is_completed", taskDataObject.IsCompleted),
            };

            ExecuteSqlCommand("Insert_Task", true, parameters);
        }

        public void Delete(IReadOnlyTaskDataObject taskDO)
        {
            if (taskDO == null)
            {
                _logger.LogError("Null task cannot be deleted.");
                return;
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", taskDO.Id)
            };

            ExecuteSqlCommand("Delete_Task", true, parameters);
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

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", taskDO.Id),
                new SqlParameter("@isCompleted", taskModel.IsCompleted)
            };

            ExecuteSqlCommand("Update_Task", true, parameters);
        }

        public List<IReadOnlyTaskDataObject> GetAll()
        {
            List<IReadOnlyTaskDataObject> results = new List<IReadOnlyTaskDataObject>();
            TaskDataObject taskDO;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select_Tasks", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            int priority = reader.GetInt32(2);
                            string text = reader.GetString(3);
                            bool isCompleted = reader.GetBoolean(4);
                            taskDO = new TaskDataObject(id, name, priority, text, isCompleted);
                            results.Add(taskDO);
                        }
                    }
                }
            }
            catch(SqlException ex)
            {
                _logger.LogError(ex, "SQL Server error.");
            }

            return results;
        }

        private void ExecuteSqlCommand(string command, bool isStoredProcedure = false, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(command, connection);

                    if (isStoredProcedure)
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        sqlCommand.Parameters.AddRange(parameters);

                    sqlCommand.ExecuteScalar();
                }
            }
            catch(SqlException sqlException)
            {
                _logger.LogError($"SQL Server error: {sqlException.Message}.");
            }
        }

        private void InitializeDatabase()
        {
            string tasksTableCreation = "IF OBJECT_ID('Tasks', 'U') IS NULL BEGIN " +
                "CREATE TABLE Tasks " +
                "(Id INT PRIMARY KEY IDENTITY(1, 1), " +
                "Name NVARCHAR(100), " +
                "Priority INT, " +
                "Text NVARCHAR(500), " +
                "Is_Completed BOOLEAN); " +
                "END";
            
            string insertTaskStoredProcedure = "IF OBJECT_ID('Insert_Task', 'P') IS NULL BEGIN " +
                "CREATE PROCEDURE Insert_Task " +
                "@id int, @name NVARCHAR(100), @priority INT, @text NVARCHAR(500) " +
                "AS " +
                "INSERT INTO TABLE Tasks(Name, Priority, Text, Is_Completed) " +
                "VALUES (@name, @priority, @text, false) " +
                "GO; " +
                "END";

            string deleteTaskStoredProcedure = "IF OBJECT_ID('Delete_Task', 'P') IS NULL BEGIN " +
                "CREATE PROCEDURE Delete_Task " +
                "@id INT " +
                "AS " +
                "DELETE FROM TABLE Tasks WHERE Id=@id " +
                "GO; " +
                "END";

            string updateTaskStoredProcedure = "IF OBJECT_ID('Update_Task', 'P') IS NULL BEGIN " +
                "CREATE PROCEDURE Update_Task " +
                "@id INT, @is_completed BOOLEAN " +
                "AS " +
                "UPDATE Tasks SET Is_Completed=@is_completed WHERE Id=@id " +
                "GO; " +
                "END";

            string selectTasksStoredProcedure = "IF OBJECT_ID('Select_Tasks', 'P') IS NULL BEGIN " +
                "CREATE PROCEDURE Select_Tasks " +
                "AS " +
                "SELECT * FROM Tasks " +
                "GO; " +
                "END";
            
            ExecuteSqlCommand(tasksTableCreation);
            ExecuteSqlCommand(insertTaskStoredProcedure);
            ExecuteSqlCommand(deleteTaskStoredProcedure);
            ExecuteSqlCommand(updateTaskStoredProcedure);
            ExecuteSqlCommand(selectTasksStoredProcedure);
        }
    }
}
