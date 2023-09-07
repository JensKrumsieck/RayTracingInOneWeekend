using System.Numerics;
using Random = Catalyze.Random;

namespace Pathtracer.Materials;

public class Dielectric: Material
{
    public float IoR = 1;
    
    public override bool Scatter(ref Ray rayIn, HitPayload payload, out Vector4 attenuation, out Ray rayOut)
    {
        attenuation = Vector4.One;
        var refractRatio = payload.FrontFace ? (1 / IoR) : IoR;
        
        var unitDir = Vector3.Normalize(rayIn.Direction);
        var cosTheta = MathF.Min(Vector3.Dot(-unitDir, payload.HitNormal), 1.0f);
        var sinTheta = MathF.Sqrt(1 - cosTheta * cosTheta);
        var cannotRefract = refractRatio * sinTheta > 1.0f || SchlickFresnel(cosTheta, refractRatio) > Random.Float(ref Pathtracer.Seed);
        var direction = cannotRefract ? 
            Reflect(unitDir, payload.HitNormal) : 
            Refract(unitDir, payload.HitNormal, refractRatio);
        rayOut = new Ray(payload.HitPoint, direction);
        return true;
    }
}
