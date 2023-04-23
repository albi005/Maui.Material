using MaterialColorUtilities.Schemes;
using Maui.Material;

namespace Playground;

public sealed class OutlinedCard : BindableMaterial
{
    private static readonly BindableProperty OutlineColorProperty = BindableProperty.Create(
        nameof(OutlineColor),
        typeof(Color),
        typeof(OutlinedCard),
        propertyChanged: (bindable, _, _) =>
        {
            ((OutlinedCard)bindable).InvalidateBackground();
        });

    public OutlinedCard()
    {
        SetDynamicResource(ColorProperty, Keys.Surface);
        SetDynamicResource(ShadowColorProperty, Keys.Shadow);
        SetDynamicResource(OutlineColorProperty, Keys.Outline);
        SetDynamicResource(StateLayerColorProperty, Keys.OnSurface);
        SetDynamicResource(SurfaceTintColorProperty, Keys.Primary);
        Interactable = true;
    }
    
    private Color? OutlineColor
    {
        get => (Color?)GetValue(OutlineColorProperty);
        set => SetValue(OutlineColorProperty, value);
    }

    protected override void DrawBackground(ICanvas canvas, RectF bounds)
    {
        base.DrawBackground(canvas, bounds);
        if (OutlineColor == null || OutlineColor.Alpha == 0) return;
        canvas.StrokeColor = OutlineColor;
        canvas.StrokeSize = 1;
        canvas.DrawRoundedRectangle(bounds, CornerRadius);
    }

    protected override void OnStateChanged(MaterialState? previousState)
    {
        base.OnStateChanged(previousState);
        Elevation = State switch
        {
            MaterialState.Hovered => 1,
            _ => 0
        };
    }
}