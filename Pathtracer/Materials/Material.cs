using System.Numerics;
using System.Runtime.CompilerServices;

namespace Pathtracer.Materials;

public abstract class Material
{
    public abstract bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut);
    public virtual Vector4 Emit(float u, float v, Vector3 at) => Vector4.Zero;
    
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
