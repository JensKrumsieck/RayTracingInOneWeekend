namespace Pathtracer;

public readonly struct Interval
{
    public readonly float Min;
    public readonly float Max;

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

    public bool Contains(float x) => Min <= x && x <= Max;
    public bool Surrounds(float x) => Min < x && x < Max;
}
