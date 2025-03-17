using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webinex.Wispo.Fcm.Devices;

public static class WispoFcmEfModelBuilderExtensions
{
    public static ModelBuilder AddWispoFcmDevices(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<WispoFcmDevice>> configure)
    {
        modelBuilder.Entity<WispoFcmDevice>(builder =>
        {
            builder.ToTable("WispoFcmDevices").HasKey(e => e.Id);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Token).HasMaxLength(2000).IsRequired();
            builder.Property(e => e.RecipientId).HasMaxLength(250).IsRequired();
            builder.Property(e => e.Meta).HasMaxLength(int.MaxValue).IsRequired(false);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasIndex(e => e.RecipientId);

            configure(builder);
        });

        return modelBuilder;
    }
}