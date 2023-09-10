namespace Pathtracer.RenderObjects;

public class HittableList : Shape
{
    public List<Hittable> Objects { get; } = new();
    public HittableList() => BoundingBox = new AABoundingBox();
    public void Add(Hittable item)
    {
        Objects.Add(item);
        BoundingBox = new AABoundingBox(BoundingBox, item.BoundingBox);
    }
    
    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        var closestT = t.Max;
        var hit = false;
        payload = HitPayload.NoHit;
        for (var i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Hit(ref ray, new Interval(t.Min, closestT), out var tmpPayload))
            {
                hit = true;
                closestT = tmpPayload.HitDistance;
                payload = tmpPayload;
            }
        }
        return hit;
    }
}
