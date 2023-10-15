using System.Numerics;

namespace Pathtracer.RenderObjects;

public struct DepthOfFieldSettings
{
    public float DefocusAngle = 0;
    public float FocusDistance = 10;
    public DepthOfFieldSettings(float defocusAngle, float focusDistance)
    {
        DefocusAngle = defocusAngle;
        FocusDistance = focusDistance;
    }
}

public class Camera
{
    public Vector3 Position = -Vector3.UnitZ;
    public Vector3 LookAt = Vector3.Zero;
    public Vector3 Up = Vector3.UnitY;
    public float VerticalFovDegrees = 90f;
    public DepthOfFieldSettings DepthOfFieldSettings = new(0, 10);
    public int SamplesPerPixel = 64;
    public int SqrtSamplesPerPixel => (int)MathF.Sqrt(SamplesPerPixel);
    private float _reciprocalSqrtSamplesPerPixel => 1f / SqrtSamplesPerPixel;

    private uint _viewportWidth;
    private uint _viewportHeight;

    private Vector3 _centerPixel;
    private Vector3 _deltaU;
    private Vector3 _deltaV;
    private Vector3 _defocusDiskU;
    private Vector3 _defocusDiskV;
    
    public void OnResize(uint width, uint height)
    {
        if(_viewportWidth == width && _viewportHeight == height) return;
        
        _viewportWidth = width;
        _viewportHeight = height;
        
        var aspectRatio = (float)_viewportWidth / _viewportHeight;

        var theta = Util.Deg2Rad(VerticalFovDegrees);
        var h = MathF.Tan(theta / 2.0f);
        var uvHeight = 2f * h * DepthOfFieldSettings.FocusDistance;
        var uvWidth = uvHeight * aspectRatio;

        var w = Vector3.Normalize(Position - LookAt);
        var u = Vector3.Normalize(Vector3.Cross(Up, w));
        var v = Vector3.Cross(w, u);
        
        var viewU = uvWidth * u;
        var viewV = uvHeight * -v;
        
        _deltaU = viewU / _viewportWidth;
        _deltaV = viewV / _viewportHeight;

        var upperLeft = Position - (DepthOfFieldSettings.FocusDistance * w) - viewU / 2 - viewV / 2;
        _centerPixel = upperLeft + .5f * (_deltaU + _deltaV);
        var defocusRadius = DepthOfFieldSettings.FocusDistance * MathF.Tan(Util.Deg2Rad(DepthOfFieldSettings.DefocusAngle / 2));
        _defocusDiskU = u * defocusRadius;
        _defocusDiskV = v * defocusRadius;
    }

    public Ray GetRay(int x, int y, int sx, int sy)
    {
       var pixelSample = _centerPixel + (x * _deltaU) + (y * _deltaV) + RandomSquare(sx, sy);
       var origin = DepthOfFieldSettings.DefocusAngle <= 0 ? Position : DefocusDiskSample();
       return new Ray(origin, pixelSample - origin);
    }


    public Vector3 DefocusDiskSample()
    {
        var p = RandomInUnitDisk();
        return Position + (p.X * _defocusDiskU) + (p.Y * _defocusDiskV);
    }

    private Vector3 RandomSquare(int sx, int sy)
    {
        var px = -.5f + _reciprocalSqrtSamplesPerPixel * (sx + Random.Float(ref Pathtracer.Seed));
        var py = -.5f + _reciprocalSqrtSamplesPerPixel * (sy + Random.Float(ref Pathtracer.Seed));
        return (px * _deltaU) + (py * _deltaV);
    }

    public float Float(ref uint seed, int min, int max)
    {
        var f = Random.Float(ref seed);
        return f * (max - min) + min;
    }
    
    private Vector3 RandomInUnitDisk()
    {
        while (true)
        {
            var p = new Vector3(Float(ref Pathtracer.Seed, -1, 1), Float(ref Pathtracer.Seed, -1, 1), 0);
            if (p.LengthSquared() < 1) return p;
        }
    }
}