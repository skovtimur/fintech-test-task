using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FintechTestTask.Infrastructure.EntitiesConfigurations;

public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.ToTable("games");

        builder.Property(x => x.OwnerId).IsRequired().HasColumnName("owner_id");
        builder.Property(x => x.RowsAndColumsnNumber).IsRequired().HasColumnName("rows_and_colums_number");
        builder.Property(x => x.CurrentTurn).HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PlayerRole>(v))
            .IsRequired().HasColumnName("current_turn");

        builder.Property(x => x.IsFinished).HasColumnName("is_finished");
        builder.Property(x => x.IsItDraw).HasColumnName("is_it_draw").HasDefaultValue(false);
        builder.Property(x => x.WinnerPlayerId).HasColumnName("winner_player_id");
        builder.Property(x => x.FinishedAt).HasConversion(BaseEntityConfiguration.Converter)
            .HasColumnName("finished_at");

        builder.HasOne(x => x.CircleUser).WithOne()
            .HasForeignKey<GameEntity>(x => x.CircleUserId)
            .HasConstraintName("FK_Game_CircleUser");

        builder.HasOne(x => x.CrossUser).WithOne()
            .HasForeignKey<GameEntity>(x => x.CrossUserId)
            .HasConstraintName("FK_Game_CrossUser");

        builder.HasIndex(g => g.OwnerId).IsUnique(false);
        builder.HasIndex(g => g.CircleUserId).IsUnique(false);
        builder.HasIndex(g => g.CrossUserId).IsUnique(false);
    }
}