namespace TennisManager.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access is denied.") { }
}
