using System.Numerics;

namespace Pathtracer.Materials.Textures;

public class NoiseTexture : Texture
{
    private readonly float _scale;
    private readonly uint _turbulence;
    private readonly float _intensity;
    private readonly bool _adjustPhase;
    public NoiseTexture(float scale, float intensity = 1.0f, uint turbulence = 0, bool adjustPhase = false)
    {
        _scale = scale;
        _turbulence = turbulence;
        _intensity = intensity;
        _adjustPhase = adjustPhase;
    }

    private readonly Perlin _perlin = new(256);
    public override Vector3 Value(float u, float v, Vector3 p)
    {
        var s = p * _scale;
        var noise = _intensity * _perlin.Noise(s, _turbulence);
        if (_adjustPhase) noise = MathF.Sin(s.Z + noise);
        return Vector3.One * 0.5f * (1 + noise);
    }
}
