using System;
using System.Linq;
using System.Collections.Generic;
using Presentation.Interfaces;

namespace Application.Implementations.UserInterfaces
{
    internal class ConsoleUserInterface : IUserInterface
    {
        private const string ExitCommand = "Exit";

        public void ChooseAction(Dictionary<string, Action> actions)
        {
            var commands = actions.Keys;
            string welcomeMessage = $"\nAvailable commands: {string.Join(", ", commands)}.\nType '{ExitCommand}' to exit.\n";
            string inputCommand;
            bool isRunning = true;
            string action;

            do
            {
                ShowMessage(welcomeMessage);
                inputCommand = ConsoleUserInputsReader.AskUserStringInput("Your command: ").ToLower();

                if (inputCommand != ExitCommand.ToLower())
                {
                    action = commands.Where(command => command.ToLower() == inputCommand).FirstOrDefault();

                    if (!string.IsNullOrEmpty(action)) actions[action]();
                    else ShowMessage("Unknown command.");
                }
                else return;
            }
            while (isRunning);
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public T AskEnumValueInput<T>(string message) where T : struct, Enum
        {
            return ConsoleUserInputsReader.AskUserEnumValueInput<T>(message);
        }

        public int AskIntInput(string message)
        {
            return ConsoleUserInputsReader.AskUserIntInput(message);
        }

        public string AskStringInput(string message)
        {
            return ConsoleUserInputsReader.AskUserStringInput(message);
        }
    }

    static internal class ConsoleUserInputsReader
    {
        static public string AskUserStringInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        static public int AskUserIntInput(string message)
        {
            bool isNumber;
            string strInput;
            int intInput;

            do
            {
                strInput = AskUserStringInput(message);
                isNumber = int.TryParse(strInput, out intInput);
            }
            while (!isNumber);

            return intInput;
        }

        static public T AskUserEnumValueInput<T>(string message) where T : struct, Enum
        {
            bool isExistingValue;
            string inputValue;
            T value;

            do
            {
                inputValue = AskUserStringInput(message);
                isExistingValue = Enum.TryParse(inputValue, out value);
                isExistingValue &= Enum.IsDefined(typeof(T), value);

                if (!isExistingValue)
                    Console.WriteLine("Incorrect value.");
            }
            while (!isExistingValue);

            return value;
        }
    }
}
