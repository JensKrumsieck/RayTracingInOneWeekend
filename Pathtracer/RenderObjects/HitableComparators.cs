namespace Pathtracer.RenderObjects;

public static class HittableComparators
{
    public static int BoxCompare(Hittable a, Hittable b, int axisIndex)
    {
        if(a.BoundingBox.Axis(axisIndex).Min < b.BoundingBox.Axis(axisIndex).Min)
            return -1;
        return 1;
    }
}

public class BoxComparatorX : IComparer<Hittable>
{
    public int Compare(Hittable a, Hittable b) => HittableComparators.BoxCompare(a, b, 0);
}
public class BoxComparatorY : IComparer<Hittable>
{
    public int Compare(Hittable a, Hittable b) => HittableComparators.BoxCompare(a, b, 0);
}
public class BoxComparatorZ : IComparer<Hittable>
{
    public int Compare(Hittable a, Hittable b) => HittableComparators.BoxCompare(a, b, 0);
}
