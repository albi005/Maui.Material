using MaterialColorUtilities.Schemes;
using Maui.Material;

namespace Playground;

public class FilledCard : BindableMaterial
{
    public FilledCard()
    {
        SetDynamicResource(ColorProperty, Keys.SurfaceVariant);
        SetDynamicResource(ShadowColorProperty, Keys.Shadow);
        SetDynamicResource(StateLayerColorProperty, Keys.OnSurfaceVariant);
        SetDynamicResource(SurfaceTintColorProperty, Keys.Primary);
        Interactable = true;
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