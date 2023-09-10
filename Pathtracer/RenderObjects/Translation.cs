using System.Numerics;

namespace Pathtracer.RenderObjects;

public class Translation : Hittable
{
    private readonly Hittable _object;
    private readonly Vector3 _offset;

    public Translation(Hittable obj, Vector3 offset)
    {
        _object = obj;
        _offset = offset;
        BoundingBox = _object.BoundingBox + _offset;
    }

    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        var offsetRay = new Ray(ray.Origin - _offset, ray.Direction);
        if (!_object.Hit(ref offsetRay, t, out payload))
            return false;
        
        payload.HitPoint += _offset;
        return true;
    }
}
