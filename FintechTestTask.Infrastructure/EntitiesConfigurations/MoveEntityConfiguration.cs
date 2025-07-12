using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FintechTestTask.Infrastructure.EntitiesConfigurations;

public class MoveEntityConfiguration : IEntityTypeConfiguration<MoveEntity>
{
    public void Configure(EntityTypeBuilder<MoveEntity> builder)
    {
        builder.ToTable("moves");
        
        builder.OwnsOne(o => o.Cell, cellBuilder =>
        {
            cellBuilder.Property(x => x.Row).IsRequired().HasColumnName("row");
            cellBuilder.Property(x => x.Column).IsRequired().HasColumnName("column");
        });
        
        builder.Property(x => x.PlayerRole).HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PlayerRole>(v))
            .IsRequired().HasColumnName("player_role");

        builder.Property(x => x.OwnerId).IsRequired().HasColumnName("owner_id");
        builder.HasOne(x => x.Owner).WithMany()
            .HasForeignKey(x => x.OwnerId).IsRequired().HasConstraintName("FK_Moves_User");

        builder.Property(x => x.GameId).IsRequired().HasColumnName("game_id");
        builder.HasOne(x => x.Game).WithMany(x => x.Moves)
            .HasForeignKey(x => x.GameId).IsRequired().HasConstraintName("FK_Moves_Game");
    }
}