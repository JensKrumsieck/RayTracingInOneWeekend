using System.Numerics;
using Pathtracer.Materials.Textures;

namespace Pathtracer.Materials;

public class Isotropic : Material
{
    private readonly Texture _albedo;
    public Isotropic(Texture albedo) => _albedo = albedo;
    public Isotropic(Vector3 color) : this(new SolidColorTexture(color)){}
    public Isotropic(float r, float g, float b) :this(new Vector3(r,g,b)){}

    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        rayOut = new Ray(payload.HitPoint, Vector3.Normalize(Random.Vec3(ref Pathtracer.Seed)));
        attenuation = new Vector4(_albedo.Value(payload.TextureCoordinate.X, payload.TextureCoordinate.Y, payload.HitPoint), 1);
        return true;
    }
}
