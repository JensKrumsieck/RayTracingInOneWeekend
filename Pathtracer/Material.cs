using System.Numerics;

namespace Pathtracer;

public abstract class Material
{
    public abstract bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut);
}
