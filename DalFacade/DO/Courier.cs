namespace DO;

using System;

/// <summary>
/// Represents a courier (delivery person) with contact details, availability, and preferred vehicle.
/// </summary>
/// <param name="Id">Unique identifier for the courier.</param>
/// <param name="Name">Courier's full name.</param>
/// <param name="PhoneNumber">Contact phone number.</param>
/// <param name="Email">Contact email address.</param>
/// <param name="Code">Internal courier code or badge identifier.</param>
/// <param name="Active">Whether the courier is currently active/available.</param>
/// <param name="TimeBegin">Time when the courier's shift or availability begins.</param>
/// <param name="typeofshipment">Preferred type of shipment/vehicle.</param>
/// <param name="Distance">Optional distance (for example, from depot) in kilometers.</param>
public record Courier
    (
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string Code,
    bool Active,
    DateTime TimeBegin,

    TypeOfShipment typeofshipment,//shiluah
    double? Distance = null

    )
{
    /// <summary>
    /// Initializes a default <see cref="Courier"/> instance with conservative default values.
    /// </summary>
    public Courier() : this(0, "", "", "", "", true, DateTime.MinValue, TypeOfShipment.Car) { }
}




