using System.Numerics;

namespace Pathtracer;

public abstract class Shape
{
    public Vector3 Position;
    public int MaterialIndex;
    
    public abstract HitPayload Hit(ref Ray ray, Interval t);
}
