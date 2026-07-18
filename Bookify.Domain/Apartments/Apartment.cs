using Bookify.Domain.Abstractions;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Apartments;

public sealed class Apartment : Entity
{
    private Apartment() { }
    public Apartment(
        Guid id,
        Name name,
        Description description,
        Address address,
        Money price,
        Money cleaningFee,
        List<Amenity> amenities)
        : base(id)
    {
        if (price.Currency != cleaningFee.Currency)
            throw new InvalidOperationException("The price and cleaning fee currencies must be the same.");

        Name = name;
        Description = description;
        Address = address;
        Price = price;
        CleaningFee = cleaningFee;
        _amenities = amenities;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Address Address { get; private set; }
    public Money Price { get; private set; }
    public Money CleaningFee { get; private set; }
    public DateTime? LastBookedOnUtc { get; internal set; }

    // حتى لو السيت برايفت بالنهايه اللست يمكن الاضافة عليها فخليها بالمره برايفت 
    //public List<Amenity> Amenities { get; private set; } = [];

    private readonly List<Amenity> _amenities = [];
    // وضيف ريد اونلي بابلك كحل
    public IReadOnlyList<Amenity> Amenities => _amenities.AsReadOnly();
}
