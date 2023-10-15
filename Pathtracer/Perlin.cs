using System.Buffers;
using System.Numerics;

namespace Pathtracer;

public sealed class Perlin
{
    private readonly Vector3[] _randVec;
    private readonly int[] _permX;
    private readonly int[] _permY;
    private readonly int[] _permZ;
    public Perlin(int size)
    {
        _randVec = new Vector3[size];
        for (var i = 0; i < size; i++) _randVec[i] = Random.InUnitSphere(ref Pathtracer.Seed);
        _permX = GeneratePerm(size);
        _permY = GeneratePerm(size);
        _permZ = GeneratePerm(size);
    }

    public float Noise(Vector3 point, uint turbulence = 0)
    {
        if (turbulence == 0) return InternalNoise(point);
        var accum = .0f;
        var temp = point;
        var weight = 1.0f;
        for (var i = 0; i < turbulence; i++)
        {
            accum += weight * InternalNoise(temp);
            weight *= .5f;
            temp *= 2f;
        }
        return MathF.Abs(accum);
    }
    
    private float InternalNoise(Vector3 point)
    {
        var u = point.X - MathF.Floor(point.X);
        var v = point.Y - MathF.Floor(point.Y);
        var w = point.Z - MathF.Floor(point.Z);

        var i = (int) MathF.Floor(point.X);
        var j = (int) MathF.Floor(point.Y);
        var k = (int) MathF.Floor(point.Z);
        var c = ArrayPool<Vector3>.Shared.Rent(8);
        for (var di = 0; di < 2; di++)
        {
            for (var dj = 0; dj < 2; dj++)
            {
                for (var dk = 0; dk < 2; dk++)
                { 
                    c[di * 4 + dj * 2 + dk] = _randVec[
                        _permX[i + di & 255] ^
                        _permY[j + dj & 255] ^
                        _permZ[k + dk & 255]
                    ];
                }
            }
        }

        var result =  Interpolate(c, u, v, w);
        ArrayPool<Vector3>.Shared.Return(c);
        return result;
    }

    private static float Interpolate(Vector3[] c, float u, float v, float w)
    {
        //hermitian smoothing
        var uu = u * u * (3 - 2 * u);
        var vv = v * v * (3 - 2 * v);
        var ww = w * w * (3 - 2 * w);
        var accum = .0f;
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                for (var k = 0; k < 2; k++)
                {
                    var weight = new Vector3(u - i, v - j, w - k);
                    accum += (i * uu + (1 - i) * (1 - uu))
                             * (j * vv + (1 - j) * (1 - vv))
                             * (k * ww + (1 - k) * (1 - ww))
                             * Vector3.Dot(c[i * 4 + j * 2 + k], weight);
                }
            }
        }

        return accum;
    }
    
    private static void Permute(int[] p, int n)
    {
        for (var i = n - 1; i > 0; i--)
        {
            var target = (int) Random.Float(ref Pathtracer.Seed, 0, i);
            (p[i], p[target]) = (p[target], p[i]);
        }
    }

    private static int[] GeneratePerm(int count)
    {
        var p = new int[count];
        for (var i = 0; i < count; i++) p[i] = i;
        Permute(p, count);
        return p;
    }
}
