using Application.Contracts;
using Application.Game.Arenas.Create;
using Domain.Game.GameModes;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Game.Arenas;

public sealed class CreateArenaCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldAcceptValidArena()
    {
        var validator = new CreateArenaCommandValidator();
        CreateArenaCommand command = CreateCommand();

        FluentValidation.Results.ValidationResult result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldRejectStartPositionOutsideGrid()
    {
        var validator = new CreateArenaCommandValidator();
        CreateArenaCommand command = CreateCommand(startPosition: new PositionDto(3, 1));

        FluentValidation.Results.ValidationResult result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.ErrorMessage == "StartPosition.X выходит за границы арены");
    }

    [Fact]
    public void Validate_ShouldRejectBlockedPositionsOutsideGrid()
    {
        var validator = new CreateArenaCommandValidator();
        CreateArenaCommand command = CreateCommand(blockedPositions: [new PositionDto(1, 1), new PositionDto(0, 2)]);

        FluentValidation.Results.ValidationResult result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.ErrorMessage == "BlockedPosition (0, 2) выходит за границы арены");
    }

    private static CreateArenaCommand CreateCommand(
        PositionDto? startPosition = null,
        List<PositionDto>? blockedPositions = null) =>
        new(
            "Training",
            GridWidth: 3,
            GridHeight: 2,
            BackgroundAsset: "arena.png",
            ImageUrl: new Uri("https://example.test/arena.png"),
            Description: "Training arena",
            GameModes: [GameModeType.Training],
            StartPosition: startPosition ?? new PositionDto(0, 0),
            BlockedPositions: blockedPositions ?? []);
}
