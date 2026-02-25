using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Core.Entities;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguation : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ReservationId(x));
        builder
            .Property(x => x.ParkingSpotId)
            .HasConversion(x => x.Value, x => new ParkingSpotId(x));
        builder.Property(x => x.Date).HasConversion(x => x.Value, x => new Date(x));
        builder
            .Property(x => x.Capacity)
            .IsRequired()
            .HasConversion(x => x.Value, x => new Capacity(x));

        builder
            .HasDiscriminator<string>("Type")
            .HasValue<CleaningReservation>(nameof(CleaningReservation))
            .HasValue<VehicleReservation>(nameof(VehicleReservation));
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));
        builder.Property(x => x.Email).HasConversion(x => x.Value, x => new Email(x));
        builder.Property(x => x.Username).HasConversion(x => x.Value, x => new Username(x));
        builder.Property(x => x.Password).HasConversion(x => x.Value, x => new Password(x));
        builder.Property(x => x.FullName).HasConversion(x => x.Value, x => new FullName(x));
        builder.Property(x => x.Role).HasConversion(x => x.Value, x => x);
    }
}
