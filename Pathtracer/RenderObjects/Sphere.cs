using System.Numerics;

namespace Pathtracer.RenderObjects;

public class Sphere : Shape
{
    public float Radius;

    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        payload = HitPayload.NoHit;
        var origin = ray.Origin - Position;
        var a = Vector3.Dot(ray.Direction, ray.Direction);
        var b = 2.0f * Vector3.Dot(origin, ray.Direction);
        var c = Vector3.Dot(origin, origin) - Radius * Radius;
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return false;
        var sqrtD = MathF.Sqrt(discriminant);
        
        var root = (-b - sqrtD)  / (2 * a);
        if (!t.Surrounds(root))
        {
            root = (-b + sqrtD) / (2 * a);
            if(!t.Surrounds(root)) return false;
        }
        var hitPoint = ray.At(root);
        var hitNormal = (hitPoint - Position) / Radius;
        
        payload = new HitPayload
        {
            HitDistance = root,
            HitPoint = hitPoint, 
            MaterialIndex =  MaterialIndex,
            TextureCoordinate = TextureCoordinates(hitNormal)
        };
        payload.SetFaceNormal(ref ray, ref hitNormal);
        return true;
    }
    public override sealed void CalculateBoundingBox() {
        
        var rVec = Radius * Vector3.One;
        BoundingBox = new AABoundingBox(Position - rVec, Position + rVec);
    }
    
    public override Vector2 TextureCoordinates(Vector3 at)
    {
        var theta = MathF.Acos(-at.Y);
        var phi = MathF.Atan2(-at.Z, at.X) + MathF.PI;
        return new Vector2(phi / MathF.Tau, theta / MathF.PI);
    }
}
