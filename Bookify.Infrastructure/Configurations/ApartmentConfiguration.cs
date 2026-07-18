using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

        builder.HasKey(apartment => apartment.Id);

        builder.OwnsOne(apartment => apartment.Address);

        builder.Property(apartment => apartment.Name)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new Name(value));

        builder.Property(apartment => apartment.Description)
            .HasMaxLength(2000)
            .HasConversion(description => description.Value, value => new Description(value));

        builder.OwnsOne(apartment => apartment.Price, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.OwnsOne(apartment => apartment.CleaningFee, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        // هذا ضفناه عشان التزامن ما يجي شخصين يعدلو على نفس الداتا بنفس الوقت فالكويري بترسل قيمه الفيرجن كشرط تحقق مشان تلغي التزامن
        // طبعا هون انا بستخدم اوبستراب داتا بيز لذلك اللي بيناسبها هو اليونيت بينما الاس كيو ال داتا بيز بيناسبها لستة من البايتز وحسب الداتا بيز يعني التايب...
        builder.Property<uint>("Version").IsRowVersion();

        builder.Ignore(apartment => apartment.Amenities);

        builder.Property<List<Amenity>>("_amenities")
            .HasColumnName("amenities")
            .HasConversion(
                new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<List<Amenity>, int[]>(
                    v => v.Select(a => (int)a).ToArray(),
                    v => v.Select(a => (Amenity)a).ToList()
                ))
            .HasColumnType("integer[]");
    }
}