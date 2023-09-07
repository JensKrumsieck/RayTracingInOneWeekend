using System.Numerics;
using SkiaSharp;

namespace Pathtracer.Materials.Textures;

public class ImageTexture : Texture
{
    private readonly SKBitmap _image;
    public ImageTexture(string filename) : this(Util.LoadImageFromFile(filename)){}
    public ImageTexture(SKBitmap image) => _image = image;
    public override Vector3 Value(float u, float v, Vector3 p)
    {
        if (_image.Height <= 0) return new Vector3(1, 0, 1);
        u = new Interval(0, 1).Clamp(u);
        v = 1.0f - new Interval(0, 1).Clamp(v); //flip v
        var i = (int) (u * _image.Width);
        var j = (int) (v * _image.Height);
        var desiredPixel = _image.GetPixel(i, j);
        const float colorScale = 1.0f / 255.0f;
        return new Vector3(colorScale * desiredPixel.Red, colorScale * desiredPixel.Green,
            colorScale * desiredPixel.Blue);
    }
}
