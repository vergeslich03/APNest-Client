using System;

namespace APNestClient.Exception;

[Serializable]
public class APNestException : System.Exception
{
    public APNestException()
    {}
    
    public APNestException(string message) : base(message)
    {}

    public APNestException(string message, System.Exception inner) : base(message, inner)
    {}
}