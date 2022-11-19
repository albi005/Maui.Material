using SkiaSharp;

namespace Maui.Material;

public class AnimatedFloat : Animated<float>
{
    public AnimatedFloat(IAnimatable animatable, Action invalidate, float initialValue, uint length)
        : base(animatable, invalidate, initialValue, length)
    {
    }

    protected override float Lerp(double progress)
        => Start + (Target - Start) * (float)progress;
}

public class AnimatedColor : Animated<SKColor>
{
    public AnimatedColor(IAnimatable animatable, Action invalidate, SKColor initialValue, uint length)
        : base(animatable, invalidate, initialValue, length)
    {
    }

    protected override SKColor Lerp(double progress) => new(
        (byte)(Start.Red + (Target.Red - Start.Red) * progress),
        (byte)(Start.Green + (Target.Green - Start.Green) * progress),
        (byte)(Start.Blue + (Target.Blue - Start.Blue) * progress),
        (byte)(Start.Alpha + (Target.Alpha - Start.Alpha) * progress)
    );
}

public class AnimatedCornerRadius : Animated<CornerRadius>
{
    public AnimatedCornerRadius(IAnimatable animatable, Action invalidate, CornerRadius initialValue, uint length)
        : base(animatable, invalidate, initialValue, length)
    {
    }

    protected override CornerRadius Lerp(double progress) => new(
        Start.TopLeft + (Target.TopLeft - Start.TopLeft) * progress,
        Start.TopRight + (Target.TopRight - Start.TopRight) * progress,
        Start.BottomLeft + (Target.BottomLeft - Start.BottomLeft) * progress,
        Start.BottomRight + (Target.BottomRight - Start.BottomRight) * progress
    );
}

public abstract class Animated<T>
{ 
    private readonly IAnimatable _animatable;
    private readonly string _id = Guid.NewGuid().ToString();
    private readonly Action _invalidate;
    private readonly uint _length;

    private T _current;
    private T _target;
    private bool _hasBeenRead;

    public Animated(
        IAnimatable animatable,
        Action invalidate,
        T initialValue,
        uint length)
    {
        _animatable = animatable;
        Start = initialValue;
        _current = initialValue;
        _target = initialValue;
        _length = length;
        _invalidate = invalidate;
    }

    public T Start { get; private set; }

    public T Current
    {
        get
        {
            _hasBeenRead = true;
            return _current;
        }
    }

    public T Target
    {
        get => _target;
        set
        {
            if (_target?.Equals(value) == true) return;

            _target = value;

            if (_hasBeenRead)
            {
                Start = _current;
                _animatable.Animate(_id, p =>
                {
                    _current = Lerp(p);
                    _invalidate();
                }, length: _length);
            }
            else
            {
                Start = value;
                _current = value;
                _invalidate();
            }
        }
    }

    protected abstract T Lerp(double progress);
}