using FakeItEasy;
using FintechTestTask.Domain.Entities;

namespace FintechTestTask.Test.ValidatorTests;

public class MoveValidator
{
    private static readonly Random _random = new();

    [Theory]
    [MemberData(nameof(GetRandomRowAndColumn))]
    public void Move_Validator_Should_Return_New_MoveEntity_Test(int row, int column)
    {
        // Arrange
        var game = A.Fake<GameEntity>();
        var user = A.Fake<UserEntity>();

        // Act
        MoveEntity? newMove = MoveEntity.Create(game, user, row, column);

        // Assert
        Assert.NotNull(newMove);
        Assert.True(Guid.TryParse(newMove.GameId.ToString(), out _));
        Assert.True(Guid.TryParse(newMove.OwnerId.ToString(), out _));
    }

    [Fact]
    public void Move_Validator_Should_Return_Null_Test()
    {
        // Arrange
        var game = A.Fake<GameEntity>();

        // Act
        MoveEntity? newMove = MoveEntity.Create(game, null, int.MinValue, -1);

        // Assert
        Assert.Null(newMove);
    }

    public static IEnumerable<object[]> GetRandomRowAndColumn()
    {
        yield return
        [
            _random.Next(3, 10), _random.Next(3, 10)
        ];
    }
}