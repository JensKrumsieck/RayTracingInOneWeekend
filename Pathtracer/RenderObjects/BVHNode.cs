namespace Pathtracer.RenderObjects;

public class BVHNode : Hittable
{
    private static readonly BoxComparatorX BoxComparatorX = new();
    private static readonly BoxComparatorY BoxComparatorY = new();
    private static readonly BoxComparatorZ BoxComparatorZ = new();
    
    private readonly Hittable _left;
    private readonly Hittable _right;

    public BVHNode(HittableList hittables) : this(hittables.Objects, 0, hittables.Objects.Count){}
    public BVHNode(IReadOnlyList<Hittable> hittables, int start, int end)
    {
        var hitObjects = hittables.ToArray();
        var axis = (int) Random.Float(ref Pathtracer.Seed, 0, 2);
        IComparer<Hittable> comparator = axis switch
        {
            0 => BoxComparatorX,
            1 => BoxComparatorY,
            _ => BoxComparatorZ
        };

        var span = end - start;
        if (span == 1) _left = _right = hitObjects[start];
        else if (span == 2)
        {
            if (comparator.Compare(hitObjects[start], hitObjects[start + 1]) < 0)
            {
                _left = hitObjects[start];
                _right = hitObjects[start + 1];
            }
            else
            {
                _left = hitObjects[start + 1];
                _right = hitObjects[start];
            }
        }
        else
        {
            Array.Sort(hitObjects, start, end-start, comparator);
            var mid = start + span / 2;
            _left = new BVHNode(hitObjects, start, mid);
            _right = new BVHNode(hitObjects, mid, end);
        }

        BoundingBox = new AABoundingBox(_left.BoundingBox, _right.BoundingBox);
    }
    
    public override bool Hit(ref Ray ray, Interval t, out HitPayload payload)
    {
        payload = HitPayload.NoHit;
        if (!BoundingBox.Hit(ref ray, t)) return false;
        var hitLeft = _left.Hit(ref ray, t, out payload);
        var hitRight = _right.Hit(ref ray, new Interval(t.Min, hitLeft ? payload.HitDistance : t.Max), out var rightPayload);
        if (hitRight) payload = rightPayload;
        return hitLeft || hitRight;
    }
}
