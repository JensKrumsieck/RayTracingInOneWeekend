using System.Numerics;

namespace Pathtracer.RenderObjects;

public abstract class Shape : Hittable
{
    public Vector3 Position;
    public int MaterialIndex;
    public abstract void CalculateBoundingBox();
    public abstract Vector2 TextureCoordinates(Vector3 at);
}
