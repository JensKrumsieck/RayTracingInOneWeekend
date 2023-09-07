using System.Numerics;

namespace Pathtracer.Materials.Textures;

public abstract class Texture
{
    public abstract Vector3 Value(float u, float v, Vector3 p);
}
