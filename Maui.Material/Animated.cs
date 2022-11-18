namespace Maui.Material;

public class Animated
{
 
    private readonly Material _material;
    private readonly string _id = Guid.NewGuid().ToString();
    private readonly Action _invalidate;
    private readonly float _min;
    private readonly float _max;
    private readonly uint _length;

    private float _current;
    private float _target;
    private bool _hasBeenUsed;

    public Animated(
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

    public float Start { get; private set; }

    public float Current
    {
        get
        {
            _hasBeenUsed = true;
            return _current;
        }
    }

    public float Target
    {
        get => _target;
        set
        {
            _target = value;

            if (_hasBeenUsed)
            {
                Start = _current;
                uint length = _length;
                // uint length = (uint)(MathF.Abs(_target - _start) / (_max - _min) * _length);
                _material.Animate(_id, p =>
                {
                    _current = (float)p;
                    _invalidate();
                }, Start, _target, length: length);
            }
            else
            {
                Start = value;
                _current = value;
                _invalidate();
            }
        }
    }
}