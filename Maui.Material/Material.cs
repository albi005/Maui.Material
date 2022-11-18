using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.Material;

public partial class Material : Layout
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(float), typeof(Material), propertyChanged: OnElevationPropertyChanged);
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(Material), propertyChanged: Invalidate);
    public static readonly BindableProperty SurfaceTintColorProperty = BindableProperty.Create(nameof(SurfaceTintColor), typeof(Color), typeof(Material), propertyChanged: Invalidate);

    private readonly SKCanvasView _canvasView;
    private readonly Animated _elevation;

    private SKPoint _touchPoint;
    private float _rippleProgressIn;
    private float _rippleProgressOut;
    private bool _prevInContact;
    private SKRoundRect? _rrect;

    public Material()
    {
        _canvasView = new() { EnableTouchEvents = true, IgnorePixelScaling = true };
        _elevation = new(this, _canvasView.InvalidateSurface, 0, 0, 12, 150);
        _canvasView.PaintSurface += Draw;
        _canvasView.Touch += OnTouch;
        Children.Add(_canvasView);
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
        Elevation = _prevInContact ? 3 : 0;
        if (_prevInContact)
        {
            _touchPoint = e.Location;
            this.AbortAnimation("rippleOut");
            _rippleProgressOut = 0;
            this.Animate("rippleIn", new Animation(p => { _rippleProgressIn = (float)p; _canvasView.InvalidateSurface(); }), 6, length: 200);
        }
        else
        {
            this.Animate("rippleOut", new Animation(p => { _rippleProgressOut = (float)p; _canvasView.InvalidateSurface(); }), length: 250);
        }
    }

    private void Draw(object? sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKRect rect = canvas.DeviceClipBounds;
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

        if (_elevation?.Current > 0 && ShadowColor != null)
        {
            SurfaceShadowData surfaceShadowData = Maui.Material.Shadow.ComputeShadow(rect, _elevation.Current)!.Value;
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
            canvas.DrawRoundRect(rrect, new() { Color = ElevationOverlay.ApplySurfaceTint(Color.ToSKColor(), SurfaceTintColor?.ToSKColor(), _elevation.Current), IsAntialias = true });

        // R I P P L E
        float endX = rect.MidX;
        float endY = rect.MidY;
        float p = MathF.Pow(_rippleProgressIn, .7f);
        float x = _touchPoint.X + (endX - _touchPoint.X) * p;
        float y = _touchPoint.Y + (endY - _touchPoint.Y) * p;
        float xDist = rect.Width / 2;
        float yDist = rect.Height / 2;
        float cornerDist = MathF.Sqrt(xDist * xDist + yDist * yDist);
        float r = p * cornerDist * 1.25f;

        SKPaint ripplePaint = new()
        {
            Shader = SKShader.CreateRadialGradient(
                new(x, y),
                r,
                new[] { SurfaceTintColor.ToSKColor().WithAlpha((byte)((1 - _rippleProgressOut) * .12f * 255)), SurfaceTintColor.ToSKColor().WithAlpha(0) },
                new[] { .05f + MathF.Pow(p, .33f) * .75f, 1f },
                SKShaderTileMode.Clamp)
        };
        canvas.ClipRoundRect(rrect);
        canvas.DrawCircle(new(x, y), r, ripplePaint);
    }

    private static void OnElevationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Material material = (Material)bindable;
        material._elevation.Target = (float)newValue;
    }
}
