namespace Maui.Material;

public abstract partial class Material : Layout
{
    private readonly GraphicsView _backgroundView;
    private readonly AnimatedFloat _elevation;
    private readonly AnimatedCornerRadius _cornerRadius;
    private readonly AnimatedColor _color;
    private readonly AnimatedColor _surfaceTintColor;
    private readonly AnimatedColor _stateOverlayColor;
    private readonly AnimatedColor _shadowColor;
    private readonly AnimatedFloat _stateOverlayOpacity;

    private GraphicsView? _touchView;
    private GraphicsView? _overlayView;
    private PointF _touchPoint;
    private float _rippleProgressIn;
    private float _rippleProgressOut;
    private MaterialState? _state;
    private bool _interactable;

    protected Material()
    {
        _backgroundView = new() { InputTransparent = true, Drawable = new BackgroundDrawable(this) };
        Add(_backgroundView);

        _elevation = new(this, _backgroundView.Invalidate, 0, 180);
        _cornerRadius = new(this, () =>
        {
            _backgroundView.Invalidate();
            _overlayView?.Invalidate();
        }, new(), 180);
        _color = new(this, _backgroundView.Invalidate, Colors.Transparent, 180);
        _surfaceTintColor = new(this, _backgroundView.Invalidate, Colors.Transparent, 180);
        _stateOverlayColor = new(this, () => _overlayView?.Invalidate(), Colors.Transparent, 180);
        _shadowColor = new(this, _backgroundView.Invalidate, Colors.Transparent, 180);
        _stateOverlayOpacity = new(this, () => _overlayView?.Invalidate(), 0, 180);
    }

    public float Elevation
    {
        get => _elevation.Current;
        set => _elevation.Target = value;
    }

    public CornerRadius CornerRadius
    {
        get => _cornerRadius.Current;
        set => _cornerRadius.Target = value;
    }

    public Color Color
    {
        get => _color.Current;
        set => _color.Target = value;
    }

    public Color SurfaceTintColor
    {
        get => _surfaceTintColor.Current;
        set => _surfaceTintColor.Target = value;
    }

    public Color StateLayerColor
    {
        get => _stateOverlayColor.Current;
        set => _stateOverlayColor.Target = value;
    }

    public Color ShadowColor
    {
        get => _shadowColor.Current;
        set => _shadowColor.Target = value;
    }

    protected MaterialState? State
    {
        get => _state;
        private set
        {
            MaterialState? prev = _state;
            _state = value;
            OnStateChanged(prev);
        }
    }

    protected bool Interactable
    {
        get => _interactable;
        set
        {
            if (_interactable == value) return;
            _interactable = value;

            if (_interactable)
            {
                _touchView = new();
                _touchView.StartHoverInteraction += (_, _) => State = MaterialState.Hovered;
                _touchView.EndHoverInteraction += (_, _) => State = null;
                _touchView.StartInteraction += (_, e) =>
                {
                    _touchPoint = e.Touches[^1];
                    State = MaterialState.Pressed;
                };
                _touchView.EndInteraction += (_, e) =>
                {
                    if (e.IsInsideBounds && State == MaterialState.Pressed) OnClick();
#if ANDROID || IOS
                    State = null;
#else
                    State = e.IsInsideBounds ? MaterialState.Hovered : null;
#endif
                };
                _touchView.CancelInteraction += (_, _) => State = null;
                Add(_touchView);

                _overlayView = new() { InputTransparent = true, ZIndex = 100, Drawable = new OverlayDrawable(this) };
                Add(_overlayView);
            }
            else
            {
                Remove(_touchView);
                Remove(_overlayView);
            }
        }
    }

    protected virtual void OnClick()
    {
        /*do nothing*/
    }

    protected virtual void OnStateChanged(MaterialState? previousState)
    {
        if (State == MaterialState.Pressed)
        {
            this.AbortAnimation("rippleOut");
            _rippleProgressOut = 0;
            this.Animate("rippleIn", p =>
            {
                _rippleProgressIn = (float)p;
                _overlayView?.Invalidate();
            }, 10, length: 400);
        }
        else if (previousState == MaterialState.Pressed)
        {
            this.Animate("rippleOut", p =>
            {
                _rippleProgressOut = (float)p;
                _overlayView?.Invalidate();
            }, 7, length: 700);
        }

        _stateOverlayOpacity.Target = State switch
        {
            MaterialState.Pressed => .12f,
            MaterialState.Focused => .12f,
            MaterialState.Hovered => .08f,
            _ => 0
        };
    }

    protected virtual void DrawBackground(ICanvas canvas, RectF bounds)
    {
        if (Color.Alpha == 0) return;
        if (Elevation > 0 && ShadowColor.Alpha != 0)
        {
            SurfaceShadowData surfaceShadowData = Maui.Material.Shadow.ComputeShadow(bounds, Elevation)!.Value;
            Color color = Maui.Material.Shadow.ToShadowColor(ShadowColor);
            canvas.SetShadow(surfaceShadowData.Offset, surfaceShadowData.BlurRadius, color);
        }

        canvas.SetFillPaint(
            ElevationOverlay.ApplySurfaceTint(Color, SurfaceTintColor, Elevation).AsPaint(),
            bounds);

        canvas.FillRoundedRectangle(bounds, CornerRadius);
    }

    protected virtual void DrawOverlay(ICanvas canvas, RectF bounds)
    {
        canvas.SetFillPaint(new SolidPaint(StateLayerColor.WithAlpha(_stateOverlayOpacity.Current)), bounds);
        canvas.FillRoundedRectangle(bounds, CornerRadius);

        if (_rippleProgressIn == 0 || _rippleProgressOut == 1) return;

        float p = MathF.Pow(_rippleProgressIn, .5f);
        float x = _touchPoint.X / bounds.Width;
        float y = _touchPoint.Y / bounds.Height;
        x += (.5f - x) * p;
        y += (.5f - y) * p;
        float r = p * 1;

        RadialGradientPaint ripplePaint = new(
            new PaintGradientStop[]
            {
                new(.5f,
                    StateLayerColor.WithAlpha((1 - _rippleProgressOut) * .12f)),
                new(1, StateLayerColor.WithAlpha(0))
            },
            new(x, y),
            r);
        canvas.SetFillPaint(ripplePaint, bounds);
        canvas.FillRoundedRectangle(bounds, CornerRadius);
    }

    protected void InvalidateBackground() => _backgroundView.Invalidate();
    protected void InvalidateOverlay() => _overlayView?.Invalidate();

    // When recycled as part of a DataTemplate, don't animate.
    protected override void OnBindingContextChanged()
    {
        _elevation.Reset();
        _cornerRadius.Reset();
        _color.Reset();
        _surfaceTintColor.Reset();
        _stateOverlayColor.Reset();
        _shadowColor.Reset();
        _stateOverlayOpacity.Reset();
        base.OnBindingContextChanged();
    }

    private sealed class BackgroundDrawable : IDrawable
    {
        private readonly Material _material;
        public BackgroundDrawable(Material material) => _material = material;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            dirtyRect = dirtyRect.Inflate(-40, -40);
            _material.DrawBackground(canvas, dirtyRect);
        }
    }

    private sealed class OverlayDrawable : IDrawable
    {
        private readonly Material _material;
        public OverlayDrawable(Material material) => _material = material;
        public void Draw(ICanvas canvas, RectF dirtyRect) => _material.DrawOverlay(canvas, dirtyRect);
    }
}