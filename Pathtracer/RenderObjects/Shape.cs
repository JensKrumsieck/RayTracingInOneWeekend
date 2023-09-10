using System.Numerics;

namespace Pathtracer.RenderObjects;

public abstract class Shape : Hittable
{
    protected Vector3 Position;
    protected int MaterialIndex;
}
