using Microsoft.Maui.Layouts;

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
                if (view == Material._backgroundView)
                    view.Arrange(bounds.Inflate(40, 40));
                else if (view == Material._touchView || view == Material._overlayView)
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
                .FirstOrDefault(x => x is not GraphicsView)?
                .Measure(widthConstraint - Layout.Padding.HorizontalThickness, heightConstraint - Layout.Padding.VerticalThickness)
                ?? new(-Layout.Padding.HorizontalThickness, -Layout.Padding.VerticalThickness);
            measure.Width += Layout.Padding.HorizontalThickness;
            measure.Height += Layout.Padding.VerticalThickness;
            return measure;
        }
    }
}
