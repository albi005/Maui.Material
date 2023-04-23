namespace Maui.Material;

public class BindableMaterial : Material
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color), typeof(Color), typeof(BindableMaterial),
        propertyChanged: (b, _, value) => ((Material)b).Color = (Color)value);

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(CornerRadius), typeof(BindableMaterial),
        propertyChanged: (b, _, value) => ((Material)b).CornerRadius = (CornerRadius)value);

    public static readonly BindableProperty SurfaceTintColorProperty = BindableProperty.Create(
        nameof(SurfaceTintColor), typeof(Color), typeof(BindableMaterial),
        propertyChanged: (b, _, value) => ((Material)b).SurfaceTintColor = (Color)value);

    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor), typeof(Color), typeof(BindableMaterial),
        propertyChanged: (b, _, value) => ((Material)b).ShadowColor = (Color)value);

    public static readonly BindableProperty StateLayerColorProperty = BindableProperty.Create(
        nameof(StateLayerColor), typeof(Color), typeof(BindableMaterial),
        propertyChanged: (b, _, value) => ((Material)b).StateLayerColor = (Color)value);

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

    public new Color? StateLayerColor
    {
        get => (Color?)GetValue(StateLayerColorProperty);
        set => SetValue(StateLayerColorProperty, value);
    }

    public new Color? ShadowColor
    {
        get => (Color?)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }
}