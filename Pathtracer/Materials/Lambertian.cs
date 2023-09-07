using System.Numerics;
using Pathtracer.Materials.Textures;
using Random = Catalyze.Random;

namespace Pathtracer.Materials;

public class Lambertian : Material
{
    public readonly Texture Albedo;

    public Lambertian(float r, float g, float b) : this(new Vector3(r,g,b)){}
    public Lambertian(Vector3 color): this(new SolidColorTexture(color)){}
    public Lambertian(Texture albedo) => Albedo = albedo;

    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        var scatterDir = payload.HitNormal + Random.InUnitSphere(ref Pathtracer.Seed);
        if (Util.DirectionNearZero(scatterDir)) scatterDir = payload.HitNormal;
        rayOut = new Ray(payload.HitPoint, scatterDir);
        attenuation = new Vector4(Albedo.Value(payload.TextureCoordinate.X, payload.TextureCoordinate.Y, payload.HitPoint), 1);
        return true;
    }
}
