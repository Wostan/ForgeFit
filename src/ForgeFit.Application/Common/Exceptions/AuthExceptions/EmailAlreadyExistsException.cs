namespace ForgeFit.Application.Common.Exceptions.AuthExceptions;

public class EmailAlreadyExistsException(string message) : Exception(message);