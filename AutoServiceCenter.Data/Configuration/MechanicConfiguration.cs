using AutoServiceCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoServiceCenter.Data.Configuration
{
    public class MechanicConfiguration : IEntityTypeConfiguration<Mechanic>
    {
        public void Configure(EntityTypeBuilder<Mechanic> entity)
        {
            entity.ToTable("Mechanics");

            entity
                .HasQueryFilter(m => m.IsDeleted == false);

            entity
                .HasOne(m => m.User)
                .WithMany() // IdentityUser has no nav to Mechanic
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}