namespace Maui.Material;

public static class Extensions
{
    public static void FillRoundedRectangle(this ICanvas canvas, Rect bounds, CornerRadius r)
        => canvas.FillRoundedRectangle(bounds, r.TopLeft, r.TopRight, r.BottomLeft, r.BottomRight);

    public static void DrawRoundedRectangle(this ICanvas canvas, Rect bounds, CornerRadius r)
        => canvas.DrawRoundedRectangle(bounds, r.TopLeft, r.TopRight, r.BottomLeft, r.BottomRight);
}