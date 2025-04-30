using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webinex.Wispo.FCM.Devices;

public static class WispoFCMEfModelBuilderExtensions
{
    public static ModelBuilder AddWispoFCMDevices(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<WispoFCMDevice>>? configure = null)
    {
        modelBuilder.Entity<WispoFCMDevice>(builder =>
        {
            builder.ToTable("WispoFCMDevices").HasKey(e => e.Id);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Token).HasMaxLength(2000).IsRequired();
            builder.Property(e => e.RecipientId).HasMaxLength(250).IsRequired();
            builder.Property(e => e.Meta).HasMaxLength(int.MaxValue).IsRequired(false);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasIndex(e => e.RecipientId);

            configure?.Invoke(builder);
        });

        return modelBuilder;
    }
}