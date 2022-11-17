using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;

namespace Maui.Material;

[ContentProperty(nameof(Content1))]
public class Material : ContentView
{
    public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(float), typeof(Material), propertyChanged: OnElevationPropertyChanged);

    private static void OnElevationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Material material = (Material)bindable;
        if (material._elevationState == null)
        {
            material._elevationState = new(
                material,
                material._canvasView.InvalidateSurface,
                (float)newValue,
                0,
                12,
                1000);
            material._canvasView.InvalidateSurface();
        }
        else
            material._elevationState.Target = (float)newValue;
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty SurfaceTintColorProperty = BindableProperty.Create(nameof(SurfaceTintColor), typeof(Color), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(Material), propertyChanged: Invalidate);

    private readonly SKCanvasView _canvasView;
    private readonly MaterialLayout _layout;

    private AnimationState? _elevationState;
    private SKPoint _touchPoint;
    private float _rippleProgressIn;
    private float _rippleProgressOut;
    private bool _prevInContact;
    private SKRoundRect? _rrect;

    public Material()
    {
        _canvasView = new() { EnableTouchEvents = true, IgnorePixelScaling = true };
        _canvasView.PaintSurface += Draw;
        _canvasView.Touch += OnTouch;
        _layout = new() { _canvasView };
        _layout.IsClippedToBounds = false;
        IsClippedToBounds = false;
        Content = _layout;
    }

    public View Content1
    {
        get => Content;
        set
        {
            _layout.RemoveAt(1);
            _layout.Add(value);
            _canvasView.InvalidateSurface();
        }
    }

    public float Elevation
    {
        get => (float)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Color? Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public Color? SurfaceTintColor
    {
        get => (Color)GetValue(SurfaceTintColorProperty);
        set => SetValue(SurfaceTintColorProperty, value);
    }
    
    public Color? ShadowColor
    {
        get => (Color?)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    private static void Invalidate(BindableObject bindableObject, object oldValue, object newValue) 
        => ((Material)bindableObject)._canvasView.InvalidateSurface();

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        e.Handled = true;
        bool isInside = _rrect!.Contains(new(e.Location.X, e.Location.Y, e.Location.X + 1, e.Location.Y + 1));
        if ((e.InContact && isInside) == _prevInContact) return;
        _prevInContact = e.InContact && isInside;
        Elevation = _prevInContact ? 3 : 12;
        if (_prevInContact)
        {
            _touchPoint = e.Location;
            this.AbortAnimation("rippleOut");
            _rippleProgressOut = 0;
            this.Animate("rippleIn", new Animation(p => { _rippleProgressIn = (float)p; _canvasView.InvalidateSurface(); }), length: 1000);
        }
        else
        {
            this.Animate("rippleOut", new Animation(p => { _rippleProgressOut = (float)p; _canvasView.InvalidateSurface(); }), length: 1500);
        }
    }

    private void Draw(object? sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKRect rect = canvas.LocalClipBounds;
        rect = new(rect.Left + 40, rect.Top + 40, rect.Right - 40, rect.Bottom - 40);

        SKRoundRect rrect = new(rect);
        rrect.SetRectRadii(rect, new[]
        {
            new SKPoint((float)CornerRadius.TopLeft, (float)CornerRadius.TopLeft),
            new SKPoint((float)CornerRadius.TopRight, (float)CornerRadius.TopRight),
            new SKPoint((float)CornerRadius.BottomRight, (float)CornerRadius.BottomRight),
            new SKPoint((float)CornerRadius.BottomLeft, (float)CornerRadius.BottomLeft),
        });
        _rrect = rrect;
        canvas.Clear();

        if (_elevationState?.Current > 0 && ShadowColor != null)
        {
            SurfaceShadowData surfaceShadowData = Maui.Material.Shadow.ComputeShadow(rect, _elevationState.Current)!.Value;
            SKRoundRect shadowBounds = new();
            shadowBounds.SetRectRadii(
                rect,
                //new(
                //    rect.Left + shadow.Spread,
                //    rect.Top + shadow.Spread,
                //    rect.Right - shadow.Spread,
                //    rect.Bottom - shadow.Spread
                //), 
                rrect.Radii);
            using SKPaint paint = new()
            {
                ImageFilter = SKImageFilter.CreateDropShadowOnly(
                    surfaceShadowData.Offset.X,
                    surfaceShadowData.Offset.Y,
                    surfaceShadowData.BlurRadius,
                    surfaceShadowData.BlurRadius,
                    Maui.Material.Shadow.ToShadowColor(ShadowColor.ToSKColor()))
            };
            canvas.DrawRoundRect(shadowBounds, paint);
        }
        
        if (Color != null)
            canvas.DrawRoundRect(rrect, new() { Color = ElevationOverlay.ApplySurfaceTint(Color.ToSKColor(), SurfaceTintColor?.ToSKColor(), _elevationState!.Current), IsAntialias = true });

        // R I P P L E
        float x = rect.Width / 2;
        float y = rect.Height / 2;
        float cornerDist = MathF.Sqrt(x * x + y * y);

        float p = _rippleProgressIn;
        p = MathF.Pow(p, .7f);

        x = _touchPoint.X + (x - _touchPoint.X) * p;
        y = _touchPoint.Y + (y - _touchPoint.Y) * p;

        float rippleRadius = (.25f + p) * cornerDist;

        SKPaint ripplePaint = new()
        {
            Shader = SKShader.CreateRadialGradient(
                new(x, y),
                rippleRadius,
                new[] { SKColors.Black.WithAlpha((byte)((1 - _rippleProgressOut) * .12f * 255)), SKColors.Black.WithAlpha(0) },
                new[] { .05f + MathF.Pow(p, .33f) * .75f, 1f },
                SKShaderTileMode.Clamp)
        };
        canvas.ClipRoundRect(rrect);
        canvas.DrawCircle(new(x, y), rippleRadius, ripplePaint);
    }
}

public class AnimationState
{
    private readonly Material _material;
    private readonly string _animationId = Guid.NewGuid().ToString();
    private readonly Action _invalidate;
    private readonly float _min;
    private readonly float _max;
    private readonly uint _length;

    private float _start;
    private float _current;
    private float _target;

    public AnimationState(
        Material material,
        Action invalidate,
        float initialValue,
        float min,
        float max,
        uint length)
    {
        _material = material;
        _current = initialValue;
        _min = min;
        _max = max;
        _length = length;
        _invalidate = invalidate;
    }

    public float Start => _start;

    public float Current => _current;

    public float Target
    {
        get => _target;
        set
        {
            _target = value;
            _start = _current;
            uint length = _length;
            // uint length = (uint)(MathF.Abs(_target - _start) / (_max - _min) * _length);
            _material.AbortAnimation(_animationId);
            _material.Animate(_animationId, p =>
            {
                _current = (float)p;
                _invalidate();
            }, _start, _target, length: length);
        }
    }
}
