using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace HumanResourceApplication.Utility
{
    public class AlreadyExistsException : ApplicationException
    {
        public AlreadyExistsException(string message) : base(message) { }
    }
    public class CustomeValidationException : ApplicationException
    {
        public IEnumerable<string> Errors { get; }
        public string TimeStamp { get; }

        public CustomeValidationException(IEnumerable<string> errors, string timeStamp)
        : base("Validation failed")
        {
            Errors = errors;
            TimeStamp = timeStamp;
        }
        
    }

    public class ExceptionFilters : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"); // Get current UTC timestamp in ISO 8601 format

            if (exception is AlreadyExistsException validationException)
            {
                context.Result = new ConflictObjectResult(new
                {
                    //Code = 409,
                    TimeStamp = timeStamp,
                    message = validationException.Message
                });
                context.ExceptionHandled = true;
            }
            else if (exception is CustomeValidationException customeValidationException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    //Code = 400,
                    TimeStamp = timeStamp,
                    message = string.Join(", ", customeValidationException.Errors)
                });
                context.ExceptionHandled = true;
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    //Code = 500,
                    TimeStamp = timeStamp,
                    message = exception.Message
                })
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
