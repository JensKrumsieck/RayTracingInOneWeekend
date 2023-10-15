using System.Numerics;
namespace Pathtracer.Materials;

public class Metal : Material
{
    public Vector3 Albedo;
    public float Roughness;

    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        var reflected = Reflect(Vector3.Normalize(rayIn.Direction), payload.HitNormal);
        attenuation = new Vector4(Albedo, 1);
        rayOut = new Ray(payload.HitPoint, reflected + Roughness * Random.InUnitSphere(ref Pathtracer.Seed));
        return Vector3.Dot(rayOut.Direction, payload.HitNormal) > 0;
    }
}
