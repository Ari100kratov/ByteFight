namespace SharedKernel;

public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string code, string message, Exception inner)
        : base(message, inner)
    {
        Code = code;
    }
}
