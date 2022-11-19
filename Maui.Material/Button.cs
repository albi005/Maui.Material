using SkiaSharp.Views.Maui;

namespace Maui.Material;

public class Button : Material
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color), typeof(Color), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).Color = ((Color)value).ToSKColor());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(CornerRadius), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).CornerRadius = (CornerRadius)value);

    public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
        nameof(Elevation), typeof(float), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).Elevation = (float)value);

    public static readonly BindableProperty SurfaceTintColorProperty = BindableProperty.Create(
        nameof(SurfaceTintColor), typeof(Color), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).SurfaceTintColor = ((Color)value).ToSKColor());

    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor), typeof(Color), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).ShadowColor = ((Color)value).ToSKColor());

    public static readonly BindableProperty ContentColorProperty = BindableProperty.Create(
        nameof(ContentColor), typeof(Color), typeof(Button),
        propertyChanged: (b, _, value) => ((Material)b).StateOverlayColor = ((Color)value).ToSKColor());

    public new float Elevation
    {
        get => (float)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public new CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public new Color? Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public new Color? SurfaceTintColor
    {
        get => (Color)GetValue(SurfaceTintColorProperty);
        set => SetValue(SurfaceTintColorProperty, value);
    }

    public Color? ContentColor
    {
        get => (Color?)GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }

    public new Color? ShadowColor
    {
        get => (Color?)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    protected override void OnStateChanged(MaterialState? previousState)
    {
        base.OnStateChanged(previousState);
        base.Elevation = State switch
        {
            MaterialState.Pressed => 12,
            MaterialState.Hovered => 1,
            _ => 0
        };
    }
}
