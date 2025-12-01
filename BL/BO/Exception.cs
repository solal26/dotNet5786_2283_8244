using System;

namespace BO;

/// <summary>
/// Base set of BL-specific exceptions used to represent business-layer errors.
/// Each exception type is semantic (does not add state) and intended to be caught/translated by callers.
/// </summary>

[Serializable]
public class BlDoesNotExistException : Exception
{
    /// <summary>Thrown when a requested business entity or resource was not found.</summary>
    public BlDoesNotExistException(string? message) : base(message) { }

    /// <summary>Thrown when a requested business entity was not found; innerException preserves lower-level details.</summary>
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    /// <summary>Thrown when attempting to create an entity that already exists.</summary>
    public BlAlreadyExistsException(string? message) : base(message) { }

    /// <summary>Thrown when attempting to create an entity that already exists; innerException preserves lower-level details.</summary>
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}


[Serializable]
public class BlInvalidValueException : Exception
{
    /// <summary>Thrown when an input value fails validation rules (range, format, etc.).</summary>
    public BlInvalidValueException(string? message) : base(message) { }

    /// <summary>Thrown when an input value fails validation rules; innerException preserves lower-level details.</summary>
    public BlInvalidValueException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidDateException : Exception
{
    /// <summary>Thrown when a date/time value violates business rules (for example: start date in the future).</summary>
    public BlInvalidDateException(string? message) : base(message) { }

    /// <summary>Thrown when a date/time value violates business rules; innerException preserves lower-level details.</summary>
    public BlInvalidDateException(string message, Exception innerException)
        : base(message, innerException) { }

}

[Serializable]
public class BlNullPropertyException : Exception
{
    /// <summary>Thrown when a required property is null or missing.</summary>
    public BlNullPropertyException(string? message) : base(message) { }

    /// <summary>Thrown when a required property is null or missing; innerException preserves lower-level details.</summary>
    public BlNullPropertyException(string message, Exception innerException)
        : base(message, innerException) { }

}

[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    /// <summary>Thrown when a requester lacks permission to perform the requested operation.</summary>
    public BlUnauthorizedAccessException(string? message) : base(message) { }

    /// <summary>Thrown when a requester lacks permission; innerException preserves lower-level details.</summary>
    public BlUnauthorizedAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidAddressException : Exception
{
    /// <summary>Thrown when address validation or geocoding fails (invalid or not found).</summary>
    public BlInvalidAddressException(string? message) : base(message) { }

    /// <summary>Thrown when address validation or geocoding fails; innerException preserves lower-level details.</summary>
    public BlInvalidAddressException(string message, Exception innerException)
        : base(message, innerException) { }

}
[Serializable]
public class BlInvalidRequestException : Exception
{
    /// <summary>Thrown when the request cannot be completed because it violates business rules or state.</summary>
    public BlInvalidRequestException(string? message) : base(message) { }

    /// <summary>Thrown when the request cannot be completed because it violates business rules or state; innerException preserves lower-level details.</summary>
    public BlInvalidRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    /// <summary>Thrown when a BL operation is temporarily unavailable (e.g., external service rate limit or maintenance).</summary>
    public BLTemporaryNotAvailableException(string? message) : base(message) { }

    /// <summary>Thrown when a BL operation is temporarily unavailable; innerException preserves lower-level details.</summary>
    public BLTemporaryNotAvailableException(string message, Exception innerException)
        : base(message, innerException) { }
}



