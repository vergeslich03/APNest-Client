namespace APNestClient.Exception;

public class ItemDeferredException : APNestException
{
    public ItemDeferredException()
    {}
    
    public ItemDeferredException(string message) : base(message)
    {}
    
    public ItemDeferredException(string message, System.Exception inner) : base(message, inner)
    {}
}