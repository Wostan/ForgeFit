namespace ForgeFit.Domain.Exceptions;

public class UnrealisticGoalException(string message) : DomainValidationException(message);