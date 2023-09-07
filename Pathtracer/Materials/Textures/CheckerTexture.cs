using System.Numerics;

namespace Pathtracer.Materials.Textures;

public class CheckerTexture : Texture
{
    private readonly Texture _even;
    private readonly Texture _odd;
    private readonly float _invScale;

    public CheckerTexture(float scale, Vector3 colorEven, Vector3 colorOdd) : this(scale,
        new SolidColorTexture(colorEven), new SolidColorTexture(colorOdd))
    { }

    public CheckerTexture(float scale, Texture even, Texture odd)
    {
        _invScale = 1.0f / scale;
        _even = even;
        _odd = odd;
    }

    public override Vector3 Value(float u, float v, Vector3 p)
    {
        var xInteger = (int) MathF.Floor(_invScale * p.X);
        var yInteger = (int) MathF.Floor(_invScale * p.Y);
        var zInteger = (int) MathF.Floor(_invScale * p.Z);
        var isEven = (xInteger + yInteger + zInteger) % 2 == 0;
        return isEven ? _even.Value(u, v, p) : _odd.Value(u, v, p);
    }
}
