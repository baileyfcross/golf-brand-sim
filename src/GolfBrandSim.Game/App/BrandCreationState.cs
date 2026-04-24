using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Game.App;

public sealed class BrandCreationState
{
    public ProductCategory SelectedSpecialization { get; set; } = ProductCategory.Equipment;

    public GameDifficulty SelectedDifficulty { get; set; } = GameDifficulty.Normal;

    public int SpecializationHoverIndex { get; set; } = -1;

    public int DifficultyHoverIndex { get; set; } = -1;

    public bool ConfirmHovered { get; set; }
}
