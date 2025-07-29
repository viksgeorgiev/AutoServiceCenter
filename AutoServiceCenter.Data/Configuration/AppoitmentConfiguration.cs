using AutoServiceCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoServiceCenter.Data.Configuration
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> entity)
        {
            entity.ToTable("Appointments");

            entity
                .HasQueryFilter(ap => ap.IsDeleted == false);

            entity
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(a => a.Vehicle)
                .WithMany() // Vehicles don’t need back-nav to appointments
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(a => a.Mechanic)
                .WithMany(m => m.Appointments)
                .HasForeignKey(a => a.MechanicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}