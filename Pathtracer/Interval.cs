namespace Pathtracer;

public readonly struct Interval
{
    public readonly float Min;
    public readonly float Max;
    public float Size => Max - Min;

    public Interval(float min, float max)
    {
        Min = min;
        Max = max;
    }
    public Interval()
    {
        Min = float.PositiveInfinity;
        Max = float.NegativeInfinity; 
    }

    public static Interval Full => new(0.0001f, float.MaxValue);

    public Interval(Interval a, Interval b)
    {
        Min = MathF.Min(a.Min, b.Min);
        Max = MathF.Max(a.Max, b.Max);
    }

    public float Clamp(float x)
    {
        if (x < Min) return Min;
        if (x > Max) return Max;
        return x;
    }

    public bool Contains(float x) => Min <= x && x <= Max;
    public bool Surrounds(float x) => Min < x && x < Max;
    public Interval Expand(float delta)
    {
        var padding = delta / 2;
        return new Interval(Min - padding, Max + padding);
    }
    public static Interval operator +(Interval i, float offset) => new(i.Min + offset, i.Max + offset);
    public static Interval operator +(float offset, Interval i) => i + offset;
}
