using System.Numerics;
using Catalyze;
using Catalyze.Applications;
using Catalyze.UI;
using Pathtracer.RenderObjects;
using Silk.NET.Vulkan;
using SkiaSharp;
using Renderer = Catalyze.Renderer;
using Texture = Catalyze.Texture;

namespace Pathtracer;

public sealed unsafe class Pathtracer : IDisposable
{
    private readonly GraphicsDevice _device = Application.GetInstance().GetModule<Renderer>()!.Device;
    private readonly ImGuiRenderer _guiRenderer = Application.GetInstance().GetModule<ImGuiRenderer>()!;

    public Settings Settings = new();
    public Texture? FinalImage => _texture;
    public int FrameIndex => _frameIndex;
    
    private Texture? _texture;
    private uint[]? _imageData;
    public static uint Seed;
    private Vector4[]? _accumulationData;

    private Scene? _activeScene;
    private Camera _activeCamera = null!;
    private int _frameIndex = 1;

    public void LoadScene(Scene scene) => _activeScene = scene.Compile();

    public void OnRender(Camera camera)
    {
        if(_texture is null || _imageData is null || _accumulationData is null) return;
        _activeCamera = camera;
        if(_frameIndex == 1) Array.Clear(_accumulationData);

        Parallel.For(0, (int) _texture.Height, y =>
        {
            for (var x = 0; x < _texture.Width; ++x)
            {
                Seed += (uint) ((x * y + _texture.Width) * _frameIndex);
                var ray = _activeCamera.GetDirection(x, y);
                _accumulationData[x + y * _texture.Width] += PerPixel(ref ray, 5);

                var color = _accumulationData[x + y * _texture.Width] / _frameIndex;
                color = Util.LinearToGamma(color);
                color = Vector4.Clamp(color, Vector4.Zero, Vector4.One);
                _imageData[x + y * _texture.Width] = Util.ToAbgr(color);
            }
        });
        
        fixed(uint* pImageData = _imageData)
            _texture.SetData(pImageData);
        
        if(Settings.Accumulate)
            _frameIndex++;
        else
            _frameIndex = 1;
    }
    private Vector4 PerPixel(ref Ray ray, int depth)
    {
        if (depth <= 0) return Vector4.Zero;
        
        var hit = _activeScene!.Hit(ref ray, Interval.Full, out var hitPayload);
        if (!hit) return RayMiss(ref ray);
        
        var material = _activeScene.Materials[hitPayload.MaterialIndex];
        var emissionColor = material.Emit(hitPayload.TextureCoordinate.X, hitPayload.TextureCoordinate.Y, hitPayload.HitPoint);
        if (!material.Scatter(ref ray, hitPayload, out var color, out var newRay))
            return emissionColor;
        var scatterColor = color * PerPixel(ref newRay, depth - 1);
        return emissionColor + scatterColor;

    }

    private Vector4 RayMiss(ref Ray ray)
    {
        var dir = Vector3.Normalize(ray.Direction);
        var u = 1 + MathF.Atan2(dir.X, -dir.Z) / MathF.PI;
        var v = MathF.Acos(dir.Y) / MathF.PI;
        var x = u / 2;
        var y = v;
        return new Vector4(_activeScene!.Background.Value(x, y, ray.Direction), 1);
    }

    public bool OnResize(uint width, uint height)
    {
        if(height == 0 || width == 0) return false;
        if (_texture is not null)
        {
            if (_texture.Width == width && _texture.Height == height)
                return false;
            _texture.Resize(width, height);
        }
        else
            _texture = new Texture(_device, width, height, Format.R8G8B8A8Srgb, ImageLayout.ShaderReadOnlyOptimal);
        
        _guiRenderer.UnloadTexture(_texture);
        _guiRenderer.LoadTexture(_texture);

        _imageData = new uint[width * height];
        _accumulationData = new Vector4[width * height];
        
        _frameIndex = 1;
        return true;
    }

    public void SaveImageToDisk()
    {
        var info = new SKImageInfo((int) _texture!.Width, (int) _texture!.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var bitmap = new SKBitmap();
        fixed (uint* pImageData = _imageData)
            bitmap.InstallPixels(info, (nint)pImageData, info.RowBytes);
        using var fs = File.Create("./render.png");
        bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
    }

    public void Reset()
    {
        Dispose();
        _texture = null;
    }
    
    public void Dispose() => _texture?.Dispose();
}
