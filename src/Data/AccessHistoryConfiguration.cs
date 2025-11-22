using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class AccessHistoryConfiguration 
    : IEntityTypeConfiguration<AccessValidationHistory>
{
    public void Configure(EntityTypeBuilder<AccessValidationHistory> builder)
    {
        builder.HasIndex(h => h.CheckedAt);

        builder
            .Property(h => h.IpAddress)
            .IsRequired();

        builder.HasIndex(h => h.IpAddress);
    }
}
