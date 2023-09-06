using System.Numerics;
using System.Runtime.CompilerServices;

namespace Pathtracer;

public struct Ray
{
    public Vector3 Origin;
    public Vector3 Direction;

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 At(float t) => Origin + t * Direction;
}
