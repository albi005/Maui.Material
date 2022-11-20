using Microsoft.Maui.Layouts;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.Material;

public partial class Material
{
    protected override ILayoutManager CreateLayoutManager() => new LayoutManager(this);

    private class LayoutManager : ILayoutManager
    {
        private readonly Material _layout;

        public LayoutManager(Material layout) => _layout = layout;

        public Size ArrangeChildren(Rect bounds)
        {
            foreach (IView view in _layout)
            {
                if (view == _layout._materialCanvasView)
                    view.Arrange(new(bounds.X - 40, bounds.Y - 40, bounds.Width + 80, bounds.Height + 80));
                else if (view == _layout._overlayCanvasView)
                    view.Arrange(bounds);
                else
                    view.Arrange(
                        new(
                            bounds.X + _layout.Padding.Left,
                            bounds.Y + _layout.Padding.Top,
                            bounds.Width - _layout.Padding.Right * 2,
                            bounds.Height - _layout.Padding.Bottom * 2));
                if (view is Material material)
                    material.ZIndex = 10;
            }

            return new(1000, 1000);
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            Size measure = _layout.Children
                .FirstOrDefault(x => x is not SKCanvasView)?
                .Measure(widthConstraint - _layout.Padding.HorizontalThickness, heightConstraint - _layout.Padding.VerticalThickness)
                ?? new(-_layout.Padding.HorizontalThickness, -_layout.Padding.VerticalThickness);
            measure.Width += _layout.Padding.HorizontalThickness;
            measure.Height += _layout.Padding.VerticalThickness;
            return measure;
        }
    }
}
