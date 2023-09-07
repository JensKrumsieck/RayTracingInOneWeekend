using System.Numerics;

namespace Pathtracer.Materials.Textures;

public class SolidColorTexture : Texture
{
    private readonly Vector3 _color;
    public SolidColorTexture(float r, float g, float b) :this(new Vector3(r,g,b)) {}
    public SolidColorTexture(Vector3 color) => _color = color;
    public override Vector3 Value(float u, float v, Vector3 p) => _color;
}
