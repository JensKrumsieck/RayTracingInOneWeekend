using System.Numerics;
using Pathtracer.Materials.Textures;

namespace Pathtracer.Materials;

public class Emissive : Material
{
    private readonly Texture _emission;
    private readonly float _power;
    public Emissive(Texture emission, float power)
    {
        _emission = emission;
        _power = power;
    }
    public Emissive(float r, float g, float b, float power) : this(new SolidColorTexture(r,g,b), power){}

    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        attenuation = Vector4.Zero;
        rayOut = rayIn;
        return false;
    }

    public override Vector4 Emit(float u, float v, Vector3 at) => new(_emission.Value(u, v, at) * _power, 1);
}
