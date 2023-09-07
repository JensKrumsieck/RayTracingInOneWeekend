namespace Pathtracer.RenderObjects;

public abstract class Hittable
{ 
    public AABoundingBox BoundingBox { get; protected set; } = null!;
    public abstract bool Hit(ref Ray ray, Interval t, out HitPayload payload);
}
