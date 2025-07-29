using AutoServiceCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoServiceCenter.Data.Configuration
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> entity)
        {
            entity.ToTable("Vehicles");

            entity
                .HasQueryFilter(v => v.IsDeleted == false);

            entity
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}