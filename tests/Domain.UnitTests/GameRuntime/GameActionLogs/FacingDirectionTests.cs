using Domain.GameRuntime.GameActionLogs;
using Shouldly;
using Xunit;

namespace Domain.UnitTests.GameRuntime.GameActionLogs;

public sealed class FacingDirectionTests
{
    [Theory]
    [InlineData(0, FacingDirection.Right)]
    [InlineData(30, FacingDirection.DownRight)]
    [InlineData(45, FacingDirection.DownRight)]
    [InlineData(90, FacingDirection.Down)]
    [InlineData(135, FacingDirection.DownLeft)]
    [InlineData(180, FacingDirection.Left)]
    [InlineData(225, FacingDirection.UpLeft)]
    [InlineData(270, FacingDirection.Up)]
    [InlineData(315, FacingDirection.UpRight)]
    [InlineData(-30, FacingDirection.UpRight)]
    [InlineData(760, FacingDirection.DownRight)] // 760 - 720 = 40 → DownRight
    public void FromAngleDegrees_ShouldSnapToSectors(double degrees, FacingDirection expected)
    {
        FacingDirectionExtensions.FromAngleDegrees(degrees).ShouldBe(expected);
    }

    [Fact]
    public void Opposite_ShouldMirrorAllDirections()
    {
        FacingDirectionExtensions.Opposite(FacingDirection.Right).ShouldBe(FacingDirection.Left);
        FacingDirectionExtensions.Opposite(FacingDirection.Down).ShouldBe(FacingDirection.Up);
        FacingDirectionExtensions.Opposite(FacingDirection.DownRight).ShouldBe(FacingDirection.UpLeft);
        FacingDirectionExtensions.Opposite(FacingDirection.UpRight).ShouldBe(FacingDirection.DownLeft);

        // Двойное отрицание возвращает исходное направление.
        foreach (FacingDirection direction in Enum.GetValues<FacingDirection>())
        {
            FacingDirectionExtensions.Opposite(
                FacingDirectionExtensions.Opposite(direction)).ShouldBe(direction);
        }
    }

    [Fact]
    public void IsRightSide_ShouldSplitScreenHalf()
    {
        FacingDirection.Right.IsRightSide().ShouldBeTrue();
        FacingDirection.DownRight.IsRightSide().ShouldBeTrue();
        FacingDirection.UpRight.IsRightSide().ShouldBeTrue();
        FacingDirection.Down.IsRightSide().ShouldBeTrue();
        FacingDirection.Up.IsRightSide().ShouldBeTrue();

        FacingDirection.Left.IsRightSide().ShouldBeFalse();
        FacingDirection.DownLeft.IsRightSide().ShouldBeFalse();
        FacingDirection.UpLeft.IsRightSide().ShouldBeFalse();
    }
}
