using System.Numerics;
using Random = Catalyze.Random;

namespace Pathtracer.RenderObjects;

public class ConstantMedium : Shape
{
    private readonly Hittable _boundary;
    private readonly float _negativeInverseDensity;

    public ConstantMedium(Hittable boundary, float d, int materialIndex)
    {
        _boundary = boundary;
        MaterialIndex = materialIndex;
        _negativeInverseDensity = -1 / d;
        BoundingBox = _boundary.BoundingBox;
    }
    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        payload = HitPayload.NoHit;
        if (!_boundary.Hit(ref ray, Interval.Full, out var payload1)) return false;
        if (!_boundary.Hit(ref ray, new Interval(payload1.HitDistance + 0.0001f, float.MaxValue), out var payload2))
            return false;
        if (payload1.HitDistance < t.Min) payload1.HitDistance = t.Min;
        if (payload2.HitDistance > t.Max) payload1.HitDistance = t.Max;
        if (payload1.HitDistance >= payload2.HitDistance) return false;
        if (payload1.HitDistance < 0) payload1.HitDistance = 0;
        var rayLength = ray.Direction.Length();
        var distanceInsideBoundary = (payload2.HitDistance - payload1.HitDistance) * rayLength;
        var hitDistance = _negativeInverseDensity * MathF.Log(Random.Float(ref Pathtracer.Seed));

        if (hitDistance > distanceInsideBoundary) return false;
        payload.HitDistance = payload1.HitDistance + hitDistance / rayLength;
        payload.HitPoint = ray.At(payload.HitDistance);
        payload.HitNormal = Vector3.UnitX;
        payload.FrontFace = true;
        payload.MaterialIndex = MaterialIndex;
        return true;
    }
}
