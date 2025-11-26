namespace BO;



[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}


[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
    public BlInvalidValueException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidDateException : Exception
{
    public BlInvalidDateException(string? message) : base(message) { }
    public BlInvalidDateException(string message, Exception innerException)
        : base(message, innerException) { }

}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
    public BlNullPropertyException(string message, Exception innerException)
        : base(message, innerException) { }

}

[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
    public BlUnauthorizedAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException(string? message) : base(message) { }
    public BlInvalidAddressException(string message, Exception innerException)
        : base(message, innerException) { }

}




