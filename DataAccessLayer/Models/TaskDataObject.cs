namespace DataAccess.Models
{
    public interface IReadOnlyTaskDataObject
    {
        int Id { get; }
        string Name { get; }
        int Priority { get; }
        string Text { get; }
        bool IsCompleted { get; }
    }

    public class TaskDataObject : IReadOnlyTaskDataObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }

        public TaskDataObject(int id, string name, int priority, string text, bool isCompleted)
        {
            Id = id;
            Name = name;
            Priority = priority;
            Text = text;
            IsCompleted = isCompleted;
        }

        public TaskDataObject() { }
    }
}
