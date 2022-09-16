using System;
using System.Configuration;
using Ninject.Modules;
using DataAccess.Interfaces;
using BusinessLogic.Interfaces;
using Presentation.Interfaces;
using Application.Implementations.Services;
using Application.Implementations.TaskManagers;
using Application.Implementations.TaskRepositories;
using Application.Implementations.UserInterfaces;

namespace Application.Utils
{
    internal class BindingsBuilder : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserInterface>().To<ConsoleUserInterface>();
            Bind<IService>().To<Service>();
            Bind<ITaskManager>().To<TaskManager>();
            Bind<ITasksRepository>().To<JsonFileTasksRepository>().WithConstructorArgument("results.json");
        }
    }
}
