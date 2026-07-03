namespace Bookify.Domain.Apartments;

public enum Amenity
{
    Wifi = 1,
    // الغلط: AirCoditioning = 2,
    AirConditioning = 2, // الصح
    Parking = 3,
    // الغلط: PetFriedly = 4,
    PetFriendly = 4, // الصح
    SwimmingPool = 5,
    Gym = 6,
    Spa = 7,
    Terrace = 8,
    MountainView = 9,
    GardenView = 10,
}
