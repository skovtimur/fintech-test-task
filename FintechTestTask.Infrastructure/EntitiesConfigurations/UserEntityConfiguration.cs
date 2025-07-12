using FintechTestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FintechTestTask.Infrastructure.EntitiesConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(24).HasColumnName("name");
        builder.Property(x => x.HashPassword).IsRequired().HasColumnName("hash_password");

        builder.Property(x => x.CurrentGameId).HasColumnName("current_game_id");
        builder.HasOne(x => x.CurrentGame).WithOne().HasForeignKey<UserEntity>(x => x.CurrentGameId)
            .HasConstraintName("FK_UserGame_Game");
    }
}