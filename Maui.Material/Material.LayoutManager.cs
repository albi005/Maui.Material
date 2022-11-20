using Microsoft.Maui.Layouts;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.Material;

public partial class Material
{
    protected override ILayoutManager CreateLayoutManager() => new MaterialLayoutManager(this);

    private class MaterialLayoutManager : LayoutManager
    {
        public MaterialLayoutManager(Microsoft.Maui.ILayout layout) : base(layout)
        {
        }

        private Material Material => (Material)Layout;

        public override Size ArrangeChildren(Rect bounds)
        {
            foreach (IView view in Layout)
            {
                if (view == Material._materialCanvasView)
                    view.Arrange(new(bounds.X - 40, bounds.Y - 40, bounds.Width + 80, bounds.Height + 80));
                else if (view == Material._touchCanvasView || view == Material._overlayCanvasView)
                    view.Arrange(bounds);
                else
                    view.Arrange(
                        new(
                            bounds.X + Layout.Padding.Left,
                            bounds.Y + Layout.Padding.Top,
                            bounds.Width - Layout.Padding.Right * 2,
                            bounds.Height - Layout.Padding.Bottom * 2));
            }

            return new();
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            Size measure = Layout
                .FirstOrDefault(x => x is not SKCanvasView)?
                .Measure(widthConstraint - Layout.Padding.HorizontalThickness, heightConstraint - Layout.Padding.VerticalThickness)
                ?? new(-Layout.Padding.HorizontalThickness, -Layout.Padding.VerticalThickness);
            measure.Width += Layout.Padding.HorizontalThickness;
            measure.Height += Layout.Padding.VerticalThickness;
            return measure;
        }
    }
}
