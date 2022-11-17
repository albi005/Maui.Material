using Microsoft.Maui.Layouts;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.Material;

public class MaterialLayout : Layout
{
    protected override ILayoutManager CreateLayoutManager() => new LayoutManager(this);

    public class LayoutManager : ILayoutManager
    {
        private readonly MaterialLayout _layout;

        public LayoutManager(MaterialLayout layout)
        {
            _layout = layout;
        }

        public Size ArrangeChildren(Rect bounds)
        {
            foreach (IView view in _layout)
            {
                if (view is SKCanvasView)
                    view.Arrange(new(bounds.X - 40, bounds.Y - 40, bounds.Width + 80, bounds.Height + 80));
                else
                    view.Arrange(new(bounds.X, bounds.Y, bounds.Width, bounds.Height));
            }

            return new();
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            return new(300, 300);
        }
    }
}