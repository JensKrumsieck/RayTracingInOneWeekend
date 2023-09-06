using System.Numerics;

namespace Pathtracer;

public struct HitPayload
{
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public float HitDistance;
    public int ObjectIndex;
    public bool FrontFace;

    public void SetFaceNormal(ref Ray ray, ref Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
        HitNormal = FrontFace ? outwardNormal : -outwardNormal;
    }

    public static readonly HitPayload NoHit = new HitPayload {HitDistance = -1f};
}
