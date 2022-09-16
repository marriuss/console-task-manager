namespace DataAccess.Models
{
    public class TaskModel
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
    }
}