using System;
using System.Collections.Generic;

namespace Presentation.Interfaces
{
    public interface IUserInterface
    {
        void ShowMessage(string message);
        void ChooseAction(Dictionary<string, Action> actions);
        string AskStringInput(string message);
        int AskIntInput(string message);
        T AskEnumValueInput<T>(string message) where T : struct, Enum;
    }
}
