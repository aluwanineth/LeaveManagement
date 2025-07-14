using FluentValidation.Results;

namespace LeaveManagement.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, List<string>>();
    }

    // Change to use a dictionary of property names and their error messages
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        foreach (var failure in failures)
        {
            // Group errors by property name
            if (!Errors.ContainsKey(failure.PropertyName))
            {
                Errors[failure.PropertyName] = new List<string>();
            }
            Errors[failure.PropertyName].Add(failure.ErrorMessage);
        }
    }
}



