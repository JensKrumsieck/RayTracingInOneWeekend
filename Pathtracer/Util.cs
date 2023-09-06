using System.Numerics;
using System.Runtime.CompilerServices;

namespace Pathtracer;

public static class Util
{ 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToAbgr(Vector4 rgba) => ToAbgr(rgba.X, rgba.Y, rgba.Z, rgba.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToAbgr(Vector3 rgba) => ToAbgr(rgba.X, rgba.Y, rgba.Z, 1);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ToAbgr(float r, float g, float b, float a = 1)
    {
        var ir = (uint) (r * 255.0f);
        var ig = (uint) (g * 255.0f);
        var ib = (uint) (b * 255.0f);
        var ia = (uint) (a * 255.0f);
        return ia << 24 | ib << 16 | ig << 8 | ir;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 LinearToGamma(Vector4 linear) => new(
        ComponentToGamma(linear.X),
        ComponentToGamma(linear.Y),
        ComponentToGamma(linear.Z),
        1);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ComponentToGamma(float linearComponent) => MathF.Pow(linearComponent, 1/2.2f);


    public static float Deg2Rad(float deg) => deg * MathF.PI / 180;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DirectionNearZero(Vector3 v) => MathF.Abs(v.X) < float.Epsilon &&
                                                       MathF.Abs(v.Y) < float.Epsilon && 
                                                       MathF.Abs(v.Z) < float.Epsilon;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Reflect(Vector3 v, Vector3 n) => v - 2 * Vector3.Dot(v, n) * n;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Refract(Vector3 uv, Vector3 n, float f0)
    {
        var cosTheta = MathF.Min(Vector3.Dot(-uv, n), 1.0f);
        var perpendicular = f0 * (uv + cosTheta * n);
        var parallel = -MathF.Sqrt(MathF.Abs(1 - perpendicular.LengthSquared())) * n;
        return perpendicular + parallel;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SchlickFresnel(float cosTheta, float ior)
    {
        var r0 = (1 - ior) / (1 + ior);
        r0 = r0 * r0;
        return r0 + (1 - r0) * MathF.Pow((1 - cosTheta), 5);
    }
}
