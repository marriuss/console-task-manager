using System;

namespace BusinessLogic.Exceptions
{
    public class ValidationException<T> : Exception
    {
        public ValidationException(T value) : base($"Forbidden value for type {typeof(T)}: {value}!") {}
    }
}
