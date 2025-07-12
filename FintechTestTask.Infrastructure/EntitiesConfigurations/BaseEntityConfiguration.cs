using FintechTestTask.Domain;
using FintechTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FintechTestTask.Infrastructure.EntitiesConfigurations;

public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public static readonly ValueConverter<DateTime, DateTime> Converter = new(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired().HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false).HasColumnName("is_deleted");
        builder.Property(e => e.DeletedAt).HasConversion(Converter).HasColumnName("deleted_at");

        builder.HasQueryFilter(e => e.IsDeleted == false);
    }
}