using System.Numerics;

namespace Pathtracer;

public class Sphere : Shape
{
    public float Radius;
    
    public override HitPayload Hit(ref Ray ray, Interval t)
    {
        var origin = ray.Origin - Position;
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = 2.0f * Vector3.Dot(origin, ray.Direction);
        var c = Vector3.Dot(origin, origin) - Radius * Radius;
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return HitPayload.NoHit;
        var sqrtD = MathF.Sqrt(discriminant);
        
        var root = (-b - sqrtD)  / (2 * a);
        if (!t.Surrounds(root))
        {
            root = (-b + sqrtD) / (2 * a);
            if(!t.Surrounds(root)) return HitPayload.NoHit;
        }
        var hitPoint = ray.At(root);
        var hitNormal = (hitPoint - Position) / Radius;
        
        var payload = new HitPayload
        {
            HitDistance = root,
            HitPoint = hitPoint
        };
        payload.SetFaceNormal(ref ray, ref hitNormal);
        
        return payload;
    }
}
