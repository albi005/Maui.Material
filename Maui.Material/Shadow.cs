namespace Maui.Material;

public static class Shadow
{
    private const float KLightHeight = 600;
    private const float KLightRadius = 800;
    private const float KLightOffsetX = -200;
    private const float KLightOffsetY = -400;

    private static SizeF ComputeShadowOffset(float elevation)
    {
        if (elevation == 0) return SizeF.Zero;

        float dx = -KLightOffsetX * elevation / KLightHeight;
        float dy = -KLightOffsetY * elevation / KLightHeight;
        return new(dx, dy);
    }

    public static Rect ComputePenumbraBounds(Rect shape, float elevation)
    {
        if (elevation == 0) return shape;

        // tangent for x
        double tx = (KLightRadius + shape.Width * .5f) / KLightHeight;
        // tangent for y
        double ty = (KLightRadius + shape.Height * .5f) / KLightHeight;
        double dx = elevation * tx;
        double dy = elevation * ty;
        SizeF offset = ComputeShadowOffset(elevation);
        
        Rect result = Rect.FromLTRB(
            shape.Left - dx,
            shape.Top - dy,
            shape.Right + dx,
            shape.Bottom + dy);
        result = result.Offset(new(offset));
        return result;
    }

    public static SurfaceShadowData? ComputeShadow(Rect shape, float elevation)
    {
        if (elevation == 0) return null;

        float penumbraTangentX = (KLightRadius + (float)shape.Width * .5f) / KLightHeight;
        float penumbraTangentY = (KLightRadius + (float)shape.Height * .5f) / KLightHeight;
        float penumbraWidth = elevation * penumbraTangentX;
        float penumbraHeight = elevation * penumbraTangentY;
        return new(Math.Min(penumbraWidth, penumbraHeight), ComputeShadowOffset(elevation));
    }

    public static Color ToShadowColor(Color color) => color.WithAlpha(color.Alpha * .3f);
}

public readonly record struct SurfaceShadowData(float BlurRadius, SizeF Offset);