using BusinessLogic.Models;

namespace Presentation.Models
{
    public class UserTask
    {
        public string Name { get; private set; }
        public Priority Priority { get; private set; }
        public string Text { get; private set; }

        public UserTask(string name, Priority priority, string text)
        {
            Name = name;
            Priority = priority;
            Text = text;
        }
    }
}
