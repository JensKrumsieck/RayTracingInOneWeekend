using System.Numerics;
using Random = Catalyze.Random;

namespace Pathtracer;

public class Lambertian : Material
{
    public Vector3 Albedo;
    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        var scatterDir = payload.HitNormal + Random.InUnitSphere(ref Pathtracer.Seed);
        if (Util.DirectionNearZero(scatterDir)) scatterDir = payload.HitNormal;
        attenuation = new Vector4(Albedo, 1);
        rayOut = new Ray(payload.HitPoint, scatterDir);
        return true;
    }
}
