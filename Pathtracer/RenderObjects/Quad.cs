using System.Numerics;

namespace Pathtracer.RenderObjects;

public class Quad : Shape
{
    public Vector3 U;
    public Vector3 V;

    public Quad(Vector3 position, Vector3 u, Vector3 v, int materialIndex)
    {
        MaterialIndex = materialIndex;
        Position = position;
        U = u;
        V = v;
        BoundingBox = new AABoundingBox(Position, Position + U + V).Pad();
    }

    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        payload = HitPayload.NoHit;
        var n = Vector3.Cross(U,V);
        var normal = Vector3.Normalize(n);
        var d = Vector3.Dot(normal, Position);
        var w = n / Vector3.Dot(n, n);
 
        var denominator = Vector3.Dot(normal, ray.Direction);
        if (MathF.Abs(denominator) < 1e-8) return false;
        
        var hitDist = (d - Vector3.Dot(normal, ray.Origin)) / denominator;
        if (!t.Contains(hitDist)) return false;
        
        var intersection = ray.At(hitDist);
        
        payload = new HitPayload
        {
            HitDistance = hitDist,
            HitPoint = intersection,
            MaterialIndex = MaterialIndex
        };
        payload.SetFaceNormal(ref ray, ref normal);
        
        var planarHitVec = intersection - Position;
        var alpha = Vector3.Dot(w, Vector3.Cross(planarHitVec, V));
        var beta = Vector3.Dot(w, Vector3.Cross(U, planarHitVec));
        return IsInterior(alpha, beta, ref payload);
    }

    private bool IsInterior(float a, float b, ref HitPayload payload)
    {
        if ((a < 0) || (1 < a) || (b < 0) || (1 < b))
            return false;
        payload.TextureCoordinate = new Vector2(a, b);
        return true;
    }
}
