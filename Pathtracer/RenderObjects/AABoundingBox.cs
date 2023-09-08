using System.Numerics;

namespace Pathtracer.RenderObjects;

public sealed class AABoundingBox
{
    private readonly Interval _x;
    private readonly Interval _y;
    private readonly Interval _z;

    public AABoundingBox()
    {
        _x = new Interval();
        _y = new Interval();
        _z = new Interval();
    }
    public AABoundingBox(Interval ix, Interval iy, Interval iz)
    { 
        _x= ix;
        _y = iy;
        _z = iz;
    }

    public AABoundingBox(Vector3 a, Vector3 b)
    {
        _x = new Interval(MathF.Min(a.X, b.X), MathF.Max(a.X, b.X));
        _y = new Interval(MathF.Min(a.Y, b.Y), MathF.Max(a.Y, b.Y));
        _z = new Interval(MathF.Min(a.Z, b.Z), MathF.Max(a.Z, b.Z));
    }

    public AABoundingBox(AABoundingBox b1, AABoundingBox b2)
    {
        _x = new Interval(b1._x, b2._x);
        _y = new Interval(b1._y, b2._y);
        _z = new Interval(b1._z, b2._z);
    }

    public AABoundingBox Pad()
    {
        var delta = .001f;
        var newX = (_x.Size >= delta) ? _x : _x.Expand(delta);
        var newY = (_y.Size >= delta) ? _y : _y.Expand(delta);
        var newZ = (_z.Size >= delta) ? _z : _z.Expand(delta);
        return new AABoundingBox(newX, newY, newZ);
    }

    public Interval Axis(int n) => n switch
    {
        1 => _y,
        2 => _z,
        _ => _x
    };

    private static void Swap(ref float x, ref float y) => (y, x) = (x, y);

    public bool Hit(ref Ray ray, Interval t)
    {
        var tMin = t.Min;
        var tMax = t.Max;
        for (var a = 0; a < 3; a++)
        {
            var invD = 1 / ray.Direction[a];
            var origin = ray.Origin[a];
            var t0 = (Axis(a).Min - origin) * invD;
            var t1 = (Axis(a).Max - origin) * invD;
            if(invD < 0) Swap(ref t0, ref t1);
            if (t0 > t.Min) tMin = t0;
            if (t1 < tMax) tMax = t1;
            if (tMax <= tMin) return false;
        }
        return true;
    }
}
