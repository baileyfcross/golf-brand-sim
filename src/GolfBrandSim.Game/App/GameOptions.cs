using Microsoft.Xna.Framework;

namespace GolfBrandSim.Game.App;

public sealed class GameOptions
{
    public static readonly Point[] SupportedResolutions =
    [
        new Point(1280, 720),
        new Point(1366, 768),
        new Point(1440, 900),
        new Point(1600, 900),
        new Point(1920, 1080)
    ];

    public int ResolutionIndex { get; set; } = 2;

    public WindowMode WindowMode { get; set; } = WindowMode.Windowed;

    public float MasterVolume { get; set; } = 0.85f;

    public float MusicVolume { get; set; } = 0.75f;

    public float SfxVolume { get; set; } = 0.80f;

    public Point SelectedResolution => SupportedResolutions[ResolutionIndex];

    public void NextResolution()
    {
        ResolutionIndex = (ResolutionIndex + 1) % SupportedResolutions.Length;
    }

    public void PreviousResolution()
    {
        ResolutionIndex = (ResolutionIndex - 1 + SupportedResolutions.Length) % SupportedResolutions.Length;
    }
}