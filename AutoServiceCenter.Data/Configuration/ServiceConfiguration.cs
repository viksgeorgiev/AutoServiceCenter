using AutoServiceCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoServiceCenter.Data.Configuration
{
    class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> entity)
        {
            entity.ToTable("Services");

            entity
                .HasQueryFilter(s => s.IsDeleted == false);
        }
    }
}