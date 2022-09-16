using DataAccess.Models;
using System.Data.Entity;

namespace DataAccess.EF
{
    public class TodoListContext : DbContext
    {
        public DbSet<TaskDataObject> Tasks { get; set; }

        public TodoListContext(string connectionString) : base(connectionString) { }
    }
}
