using System.Numerics;

namespace Pathtracer.RenderObjects;

public class RotationY : Hittable
{
    private readonly Hittable _object;
    private readonly float _cosTheta;
    private readonly float _sinTheta;

    public RotationY(Hittable o, float angle)
    {
        _object = o;
        var rad = Util.Deg2Rad(angle);
        _cosTheta = MathF.Cos(rad);
        _sinTheta = MathF.Sin(rad);
        CalculateBoundingBox();
    }

    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        var ori = ray.Origin;
        var dir = ray.Direction;
        ori.X = _cosTheta * ray.Origin.X - _sinTheta * ray.Origin.Z;
        ori.Z = _sinTheta * ray.Origin.X + _cosTheta * ray.Origin.Z;
        
        dir.X = _cosTheta * ray.Direction.X - _sinTheta * ray.Direction.Z;
        dir.Z = _sinTheta * ray.Direction.X + _cosTheta * ray.Direction.Z;
        var rotatedRay = new Ray(ori, dir);
        if (!_object.Hit(ref rotatedRay, t, out payload)) return false;
        
        var p = payload.HitPoint;
        p.X = _cosTheta * payload.HitPoint.X + _sinTheta * payload.HitPoint.Z;
        p.Z = -_sinTheta * payload.HitPoint.X + _cosTheta * payload.HitPoint.Z;


        var n = payload.HitNormal;
        n.X = _cosTheta * payload.HitNormal.X + _sinTheta * payload.HitNormal.Z;
        n.Z = -_sinTheta * payload.HitNormal.X + _cosTheta * payload.HitNormal.Z;

        payload.HitPoint = p;
        payload.HitNormal = n;
        return true;
    }
    public void CalculateBoundingBox()
    {
        var bbox = _object.BoundingBox;
        var min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        var max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                for (var k = 0; k < 2; k++)
                {
                    var x = i * bbox.X.Max + (1 - i) * bbox.X.Min;
                    var y = j* bbox.Y.Max + (1 - j) * bbox.Y.Min;
                    var z = k * bbox.Z.Max + (1 - k) * bbox.Z.Min;
                    var newX = _cosTheta * x + _sinTheta * z;
                    var newZ = -_sinTheta * x + _cosTheta * z;
                    var tester = new Vector3(newX, y, newZ);
                    for (var c = 0; c < 3; c++)
                    {
                        min[c] = MathF.Min(min[c], tester[c]);
                        max[c] = MathF.Max(max[c], tester[c]);
                    }
                }
            }
        }
        BoundingBox = new AABoundingBox(min, max);
    }
}
