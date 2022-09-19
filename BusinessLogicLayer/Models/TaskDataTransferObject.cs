namespace BusinessLogic.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High,
    }

    public class TaskDataTransferObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Priority Priority { get; private set; }
        public string Text { get; private set; }
        public bool IsCompleted { get; private set; }

        public TaskDataTransferObject() { }
    }
}
