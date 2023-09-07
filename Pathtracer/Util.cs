using System.Numerics;
using System.Runtime.CompilerServices;
using SkiaSharp;

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
    
    //obsolete with next update of Catalyze
    public static SKBitmap LoadImageFromFile(string filename)
    {
        using var fs = File.Open(filename, FileMode.Open);
        using var codec = SKCodec.Create(fs);
        var imgInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var image = SKBitmap.Decode(codec, imgInfo);
        if (image is null) throw new IOException($"Failed to load image from {filename}");
        return image;
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

}
