using System.Numerics;

namespace Pathtracer;

public class Onb
{
    private Vector3[] _axis = new Vector3[3];

    public Onb(Vector3 w) => BuildFromW(w);

    public Vector3 U => _axis[0];
    public Vector3 V => _axis[1];
    public Vector3 W => _axis[2];

    public Vector3 this[int i] => _axis[i];
    
    public Vector3 Local(float a, float b, float c) => a * U + b * V + c * W;
    public Vector3 Local(Vector3 a) => Local(a.X, a.Y, a.Z);
    
    public void BuildFromW(Vector3 w)
    {
        var unitW = Vector3.Normalize(w);
        var a = MathF.Abs(unitW.X) > .9f ? Vector3.UnitY : Vector3.UnitX;
        var v = Vector3.Normalize(Vector3.Cross(unitW, a));
        var u = Vector3.Cross(unitW, v);
        _axis[0] = u;
        _axis[1] = v;
        _axis[2] = unitW;
    }
}
