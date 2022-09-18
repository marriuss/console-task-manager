using System;
using System.Linq;
using System.Collections.Generic;
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
            BindInterfaceImplementation<IUserInterface>();
            BindInterfaceImplementation<IService>();
            BindInterfaceImplementation<ITaskManager>();
            BindInterfaceImplementation<ITasksRepository>();
        }

        private void BindInterfaceImplementation<T>()
        {
            Type implementation = ImplementationsInfo.GetImplementation<T>();
            Bind<T>().To(implementation);
        }
    }

    internal static class ImplementationsInfo
    {
        private static List<InterfaceImplementationInfo> _implementations =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(InterfaceImplementationInfo).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => (InterfaceImplementationInfo)Activator.CreateInstance(x)).ToList();

        public static Type GetImplementation<T>()
        {
            Type interfaceType = typeof(T);
            InterfaceImplementationInfo implementationInfo = _implementations.Find(implementation => implementation.InterfaceType == interfaceType);

            if (implementationInfo != null)
            {
                string value = ConfigurationManager.AppSettings.Get(implementationInfo.ConfigKey);
                return string.IsNullOrEmpty(value) ? implementationInfo.DefaultImplementation : implementationInfo.GetImplementation(value.ToLower());
            }
            else throw new ApplicationException($"Unknown type: {interfaceType}.");
        }
    }

    internal abstract class InterfaceImplementationInfo
    {
        public abstract Type InterfaceType { get; }
        public abstract string ConfigKey { get; }
        public abstract Type DefaultImplementation { get; }

        public Type GetImplementation(string value)
        {
            Dictionary<string, Type> implementationValues = GetImplementations();
            return implementationValues.ContainsKey(value) ? implementationValues[value] : DefaultImplementation;
        }

        protected abstract Dictionary<string, Type> GetImplementations();
    }

    internal sealed class ServiceImplementationInfo : InterfaceImplementationInfo
    {
        public override Type InterfaceType => typeof(IService);
        public override string ConfigKey => "service";
        public override Type DefaultImplementation => typeof(Service);

        protected override Dictionary<string, Type> GetImplementations()
        {
            return new Dictionary<string, Type>()
            {
                { "service", typeof(Service) }
            };
        }
    }

    internal sealed class TaskManagerImplementationInfo : InterfaceImplementationInfo
    {
        public override Type InterfaceType => typeof(ITaskManager);
        public override string ConfigKey => "taskmanager";
        public override Type DefaultImplementation => typeof(TaskManager);

        protected override Dictionary<string, Type> GetImplementations()
        {
            return new Dictionary<string, Type>()
            {
                { "taskManager", typeof(TaskManager) }
            };
        }
    }

    internal sealed class TaskRepositoryImplementationInfo : InterfaceImplementationInfo
    {
        public override Type InterfaceType => typeof(ITasksRepository);
        public override string ConfigKey => "taskrepository";
        public override Type DefaultImplementation => typeof(JsonFileTasksRepository);

        protected override Dictionary<string, Type> GetImplementations()
        {
            return new Dictionary<string, Type>()
            {
                { "json", typeof(JsonFileTasksRepository) },
                { "database", typeof(DatabaseTaskRepository) },
            };
        }
    }

    internal sealed class UserInterfaceImplementationInfo : InterfaceImplementationInfo
    {
        public override Type InterfaceType => typeof(IUserInterface);
        public override string ConfigKey => "userinterface";
        public override Type DefaultImplementation => typeof(ConsoleUserInterface);

        protected override Dictionary<string, Type> GetImplementations()
        {
            return new Dictionary<string, Type>()
            {
                { "console", typeof(ConsoleUserInterface) }
            };
        }
    }

}
