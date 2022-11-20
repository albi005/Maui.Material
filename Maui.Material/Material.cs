using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;

namespace Maui.Material;

public abstract partial class Material : Layout
{
    private readonly SKCanvasView _materialCanvasView;
    private readonly SKCanvasView _touchCanvasView;
    private readonly SKCanvasView _overlayCanvasView;
    private readonly AnimatedFloat _elevation;
    private readonly AnimatedCornerRadius _cornerRadius;
    private readonly AnimatedColor _color;
    private readonly AnimatedColor _surfaceTintColor;
    private readonly AnimatedColor _stateOverlayColor;
    private readonly AnimatedColor _shadowColor;
    private readonly AnimatedFloat _stateOverlayOpacity;
    private readonly SKPoint[] _cornerRadii = new SKPoint[4];

    private SKPoint _touchPoint;
    private float _rippleProgressIn;
    private float _rippleProgressOut;

    public Material()
    {        
        _materialCanvasView = new() { IgnorePixelScaling = true, InputTransparent = true };
        _materialCanvasView.PaintSurface += (_, e) =>
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Translate(40, 40);
            DrawBackground(canvas);
        };
        Children.Add(_materialCanvasView);

        _touchCanvasView = new() { EnableTouchEvents = true, IgnorePixelScaling = true };
        _touchCanvasView.Touch += OnTouch;
        Children.Add(_touchCanvasView);

        _overlayCanvasView = new() { InputTransparent = true, IgnorePixelScaling = true, ZIndex = 100 };
        _overlayCanvasView.PaintSurface += (_, e) => DrawOverlay(e.Surface.Canvas);
        Children.Add(_overlayCanvasView);

        _elevation = new(this, _materialCanvasView.InvalidateSurface, 0, 180);
        _cornerRadius = new(this, () =>
        {
            static SKPoint CreatePoint(double xy) => new((float)xy, (float)xy);
            _cornerRadii[0] = CreatePoint(CornerRadius.TopRight);
            _cornerRadii[1] = CreatePoint(CornerRadius.TopLeft);
            _cornerRadii[2] = CreatePoint(CornerRadius.BottomRight);
            _cornerRadii[3] = CreatePoint(CornerRadius.BottomLeft);
            UpdateBoundingRect();
            _materialCanvasView.InvalidateSurface();
            _overlayCanvasView.InvalidateSurface();
        }, new(), 180);
        _color = new(this, _materialCanvasView.InvalidateSurface, SKColor.Empty, 180);
        _surfaceTintColor = new(this, _materialCanvasView.InvalidateSurface, SKColor.Empty, 180);
        _stateOverlayColor = new(this, _overlayCanvasView.InvalidateSurface, SKColor.Empty, 180);
        _shadowColor = new(this, _materialCanvasView.InvalidateSurface, SKColor.Empty, 180);
        _stateOverlayOpacity = new(this, _overlayCanvasView.InvalidateSurface, 0, 180);
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

    public SKColor Color
    {
        get => _color.Current;
        set => _color.Target = value;
    }

    public SKColor SurfaceTintColor
    {
        get => _surfaceTintColor.Current;
        set => _surfaceTintColor.Target = value;
    }

    public SKColor StateOverlayColor
    {
        get => _stateOverlayColor.Current;
        set => _stateOverlayColor.Target = value;
    }
    
    public SKColor ShadowColor
    {
        get => _shadowColor.Current;
        set => _shadowColor.Target = value;
    }

    protected SKRoundRect BoundingRect { get; } = new();

    protected MaterialState? State { get; private set; }

    protected virtual void OnTouch(object? sender, SKTouchEventArgs e)
    {
        MaterialState? prevState = State;
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                State = MaterialState.Pressed;
                _touchPoint = e.Location;
                e.Handled = true;
                break;
            case SKTouchAction.Entered:
                State = MaterialState.Hovered;
                e.Handled = true;
                break;
            case SKTouchAction.Released:
                if (State == MaterialState.Pressed)
                {
                    Debug.WriteLine("CLICKED!");
                    // TODO: Handle released
                }
                State = e.DeviceType != SKTouchDeviceType.Touch
                    ? MaterialState.Hovered
                    : null;
                e.Handled = true;
                break;
            case SKTouchAction.Cancelled:
            case SKTouchAction.Exited:
                State = null;
                break;
            case SKTouchAction.Moved:
                if (SKPoint.Distance(_touchPoint, e.Location) > 15)
                    State = e.DeviceType != SKTouchDeviceType.Touch
                        ? MaterialState.Hovered
                        : null;
                break;
            case SKTouchAction.WheelChanged:
            default:
                break;
        }

        if (prevState != State) OnStateChanged(prevState);
    }

    protected virtual void OnStateChanged(MaterialState? previousState)
    {
        if (State == MaterialState.Pressed)
        {
            this.AbortAnimation("rippleOut");
            _rippleProgressOut = 0;
            this.Animate("rippleIn", p => { _rippleProgressIn = (float)p; _overlayCanvasView.InvalidateSurface(); }, 6, length: 200);
        }
        else if (previousState == MaterialState.Pressed)
        {
            this.Animate("rippleOut", p => { _rippleProgressOut = (float)p; _overlayCanvasView.InvalidateSurface(); }, length: 400);
        }

        _stateOverlayOpacity.Target = State switch
        {
            MaterialState.Pressed => .12f,
            MaterialState.Focused => .12f,
            MaterialState.Hovered => .08f,
            _ => 0
        };
    }

    protected virtual void DrawBackground(SKCanvas canvas)
    {
        canvas.Clear();

        if (Elevation > 0 && ShadowColor.Alpha != 0)
        {
            SurfaceShadowData surfaceShadowData = Maui.Material.Shadow.ComputeShadow(BoundingRect.Rect, Elevation)!.Value;
            using SKPaint paint = new()
            {
                ImageFilter = SKImageFilter.CreateDropShadowOnly(
                    surfaceShadowData.Offset.X,
                    surfaceShadowData.Offset.Y,
                    surfaceShadowData.BlurRadius,
                    surfaceShadowData.BlurRadius,
                    Maui.Material.Shadow.ToShadowColor(ShadowColor))
            };
            canvas.DrawRoundRect(BoundingRect, paint);
        }
        
        if (Color.Alpha != 0)
        {
            canvas.DrawRoundRect(
                BoundingRect,
                new()
                {
                    Color = ElevationOverlay.ApplySurfaceTint(Color, SurfaceTintColor, Elevation),
                    IsAntialias = true
                }
            );
        }
    }

    protected virtual void DrawOverlay(SKCanvas canvas)
    {
        canvas.Clear();
        canvas.DrawRoundRect(BoundingRect, new() { Color = StateOverlayColor.WithAlpha((byte)(_stateOverlayOpacity.Current * 255)) });

        if (_rippleProgressIn == 0) return;

        SKRect rect = BoundingRect.Rect;
        float endX = rect.MidX;
        float endY = rect.MidY;
        float p = _rippleProgressIn;
        float x = _touchPoint.X + (endX - _touchPoint.X) * p;
        float y = _touchPoint.Y + (endY - _touchPoint.Y) * p;
        float xDist = rect.Width / 2;
        float yDist = rect.Height / 2;
        float cornerDist = MathF.Sqrt(xDist * xDist + yDist * yDist);
        float r = MathF.Pow(p, .7f) * cornerDist * 1.25f;

        SKPaint ripplePaint = new()
        {
            Shader = SKShader.CreateRadialGradient(
                new(x, y),
                r,
                new[] { StateOverlayColor.WithAlpha((byte)((1 - _rippleProgressOut) * .12f * 255)), StateOverlayColor.WithAlpha(0) },
                new[] { .05f + MathF.Pow(p, .33f) * .75f, 1f },
                SKShaderTileMode.Clamp)
        };

        canvas.ClipRoundRect(BoundingRect);
        canvas.DrawCircle(new(x, y), r, ripplePaint);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateBoundingRect();
    }

    private void UpdateBoundingRect()
    {
        BoundingRect.SetRectRadii(
            new(0, 0, (float)Bounds.Width, (float)Bounds.Height),
            _cornerRadii
        );
    }
}
