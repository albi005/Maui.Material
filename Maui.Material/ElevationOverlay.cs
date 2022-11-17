using MaterialColorUtilities.Utils;
using SkiaSharp;

namespace Maui.Material;

public static class ElevationOverlay
{
    private static readonly ElevationOpacity[] _surfaceTintElevationOpacities =
    {
        new(0, 0),
        new(1, .05f),
        new(3, 0.08f),
        new(6, .11f),
        new(8, .12f),
        new(12, .14f)
    };

    public static SKColor ApplySurfaceTint(SKColor color, SKColor? surfaceTint, float elevation)
        => surfaceTint != null ? ((uint)color).Add((uint)surfaceTint, SurfaceTintOpacityForElevation(elevation)) : color;

    private static float SurfaceTintOpacityForElevation(float elevation)
    {
        if (elevation < _surfaceTintElevationOpacities[0].Elevation) {
            // Elevation less than the first entry, so just clamp it to the first one.
            return _surfaceTintElevationOpacities[0].Opacity;
        }

        // Walk the opacity list and find the closest match(es) for the elevation.
        int index = 0;
        while (elevation >= _surfaceTintElevationOpacities[index].Elevation) {
            // If we found it exactly or walked off the end of the list just return it.
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (elevation == _surfaceTintElevationOpacities[index].Elevation ||
                index + 1 == _surfaceTintElevationOpacities.Length) {
                return _surfaceTintElevationOpacities[index].Opacity;
            }
            index += 1;
        }

        // Interpolate between the two opacity values
        ElevationOpacity lower = _surfaceTintElevationOpacities[index - 1];
        ElevationOpacity upper = _surfaceTintElevationOpacities[index];
        float t = (elevation - lower.Elevation) / (upper.Elevation - lower.Elevation);
        return lower.Opacity + t * (upper.Opacity - lower.Opacity);
    }

    private record ElevationOpacity(float Elevation, float Opacity);
}
