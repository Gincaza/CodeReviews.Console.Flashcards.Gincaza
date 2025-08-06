using System.Reflection;

namespace Business_Logic.Messages;
public class OperationResult
{
    public bool Success;
    public string? Message;

    public OperationResult(bool success, string? message)
    {
        this.Success = success; 
        this.Message = message;
    }
}