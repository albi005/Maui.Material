using MaterialColorUtilities.Schemes;
using Maui.Material;

namespace Playground;

public class FilledButton : BindableMaterial
{
    public FilledButton()
    {
        SetDynamicResource(ColorProperty, Keys.Primary);
        SetDynamicResource(ShadowColorProperty, Keys.Shadow);
        SetDynamicResource(StateLayerColorProperty, Keys.OnPrimary);
        SetDynamicResource(SurfaceTintColorProperty, Keys.Primary);
        Interactable = true;
        HeightRequest = 40;
        Padding = new(24, 0);
        CornerRadius = 20;
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