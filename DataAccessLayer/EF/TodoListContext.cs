using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DataAccess.Models;

namespace DataAccess
{
    internal class TodoListContext : DbContext
    {
        public DbSet<TaskDataObject> Tasks { get; set; }

        public TodoListContext(string connectionString) : base(connectionString) { }
    }
}
