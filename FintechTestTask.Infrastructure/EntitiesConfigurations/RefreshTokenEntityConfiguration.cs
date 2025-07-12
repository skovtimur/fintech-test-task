using FintechTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FintechTestTask.Infrastructure.EntitiesConfigurations;

public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.Property(e => e.TokenHash)
            .IsRequired()
            .HasColumnName("token_hash");
        builder.HasIndex(e => e.TokenHash).IsUnique();


        builder.Property(x => x.ExpiresAtUtc).HasConversion(BaseEntityConfiguration.Converter)
            .IsRequired().HasColumnName("expires_at");

        builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
        builder.HasOne(x => x.User).WithOne()
            .HasForeignKey<RefreshTokenEntity>(x => x.UserId)
            .IsRequired().HasConstraintName("FK_RefreshToken_User").OnDelete(DeleteBehavior.Cascade);
    }
}