using SkiaSharp;

namespace Maui.Material;

public static class Shadow
{
    private const float KLightHeight = 600;
    private const float KLightRadius = 800;
    private const float KLightOffsetX = -200;
    private const float KLightOffsetY = -400;

    private static SKPoint ComputeShadowOffset(float elevation)
    {
        if (elevation == 0) return SKPoint.Empty;

        float dx = -KLightOffsetX * elevation / KLightHeight;
        float dy = -KLightOffsetY * elevation / KLightHeight;
        return new(dx, dy);
    }

    public static SKRect ComputePenumbraBounds(SKRect shape, float elevation)
    {
        if (elevation == 0) return shape;

        // tangent for x
        float tx = (KLightRadius + shape.Width * .5f) / KLightHeight;
        // tangent for y
        float ty = (KLightRadius + shape.Height * .5f) / KLightHeight;
        float dx = elevation * tx;
        float dy = elevation * ty;
        SKPoint offset = ComputeShadowOffset(elevation);
        
        SKRect result = SKRect.Create(
            shape.Left - dx,
            shape.Top - dy,
            shape.Width + dx,
            shape.Height + dy);
        result.Offset(offset);
        return result;
    }

    public static SurfaceShadowData? ComputeShadow(SKRect shape, float elevation)
    {
        if (elevation == 0) return null;

        float penumbraTangentX = (KLightRadius + shape.Width * .5f) / KLightHeight;
        float penumbraTangentY = (KLightRadius + shape.Height * .5f) / KLightHeight;
        float penumbraWidth = elevation * penumbraTangentX;
        float penumbraHeight = elevation * penumbraTangentY;
        return new(Math.Min(penumbraWidth, penumbraHeight), ComputeShadowOffset(elevation));
    }

    public static SKColor ToShadowColor(SKColor color) => color.WithAlpha((byte)(color.Alpha * .3f));
}

public readonly record struct SurfaceShadowData(float BlurRadius, SKPoint Offset);