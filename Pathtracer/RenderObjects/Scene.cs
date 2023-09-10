using System.Numerics;
using Pathtracer.Materials;
using Pathtracer.Materials.Textures;
using Random = Catalyze.Random;

namespace Pathtracer.RenderObjects;

public class Scene : HittableList
{
    public readonly List<Material> Materials = new();
    public Texture Background = new SolidColorTexture(.1f, .1f, .1f);
    
    public Scene Compile()
    {
        var scene = new Scene();
        scene.Objects.Add(new BVHNode(this));
        scene.Materials.AddRange(Materials);
        scene.BoundingBox = BoundingBox;
        scene.Background = Background;
        return scene;
    }

    public static HittableList MakeBox(Vector3 a, Vector3 b, int materialIndex)
    {
        var sides = new HittableList();
        var min = new Vector3(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y), MathF.Min(a.Z, b.Z));
        var max = new Vector3(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y), MathF.Max(a.Z, b.Z));
        var dx = new Vector3(max.X - min.X, 0, 0);
        var dy = new Vector3(0, max.Y - min.Y, 0);
        var dz = new Vector3(0, 0, max.Z - min.Z);

        sides.Add(new Quad (new Vector3(min.X, min.Y, max.Z), dx,  dy,  materialIndex));
        sides.Add(new Quad (new Vector3(max.X, min.Y, max.Z), -dz,  dy,  materialIndex));
        sides.Add(new Quad (new Vector3(max.X, min.Y, min.Z), -dx,  dy,  materialIndex));
        sides.Add(new Quad (new Vector3(min.X, min.Y, min.Z), dz,  dy,  materialIndex));
        sides.Add(new Quad (new Vector3(min.X, max.Y, max.Z), dx, -dz,  materialIndex));
        sides.Add(new Quad (new Vector3(min.X, min.Y, min.Z), dx,  dz,  materialIndex));
        return sides;
    }

    public static Scene CornellBox()
    {
        var scene = new Scene();
        scene.Background = new SolidColorTexture(0, 0, 0);
        scene.Materials.Add(new Lambertian(.65f, .05f, .05f));
        scene.Materials.Add(new Lambertian(.73f, .73f, .73f));
        scene.Materials.Add(new Lambertian(.12f, .45f, .15f));
        scene.Materials.Add(new Emissive(1, 1, 1, 15));
        
        scene.Objects.Add(new Quad(new Vector3(555,0,0), new Vector3(0,555,0),  new Vector3(0,0,555),  2));
        scene.Objects.Add(new Quad(new Vector3(0,0,0), new Vector3(0,555,0),  new Vector3(0,0,555),  0));
        scene.Objects.Add(new Quad(new Vector3(343,554,332), new Vector3(-130,0,0),  new Vector3(0,0,-105),  3));
        scene.Objects.Add(new Quad(new Vector3(0,0,0), new Vector3(555,0,0),  new Vector3(0,0,555),  1));
        scene.Objects.Add(new Quad(new Vector3(555,555,555), new Vector3(-555,0,0),  new Vector3(0,0,-555),  1));
        scene.Objects.Add(new Quad(new Vector3(0,0,555), new Vector3(555,0,0),  new Vector3(0,555,0),  1));
        
        var box1 = (Hittable)MakeBox(new Vector3(0,0,0), new Vector3(165,330,165), 1);
        box1 = new RotationY(box1, 15);
        box1 = new Translation(box1, new Vector3(265, 0, 295));
        scene.Add(box1);
        
        var box2 = (Hittable)MakeBox(new Vector3(0,0,0), new Vector3(165,165,165), 1);
        box2 = new RotationY(box2, -18);
        box2 = new Translation(box2, new Vector3(130, 0, 65));
        scene.Add(box2);
        
        return scene;
    }
    
    public static Scene Quads()
    {
        var scene = new Scene();
        scene.Materials.Add(new Lambertian(1f,.2f,.2f));
        scene.Materials.Add(new Lambertian(.2f,1f,.2f));
        scene.Materials.Add(new Lambertian(.2f,.2f,1f));
        scene.Materials.Add(new Lambertian(1f,.5f,.0f));
        scene.Materials.Add(new Lambertian(.2f,.8f,.8f));
        
        scene.Objects.Add(new Quad(new Vector3(-3,-2,5),  new Vector3(0,0,-4),  new Vector3(0,4,0),  0));
        scene.Objects.Add(new Quad(new Vector3(-2,-2,0),  new Vector3(4,0,0),  new Vector3(0,4,0),  1));
        scene.Objects.Add(new Quad(new Vector3(3,-2,1),  new Vector3(0,0,4),  new Vector3(0,4,0),  2));
        scene.Objects.Add(new Quad(new Vector3(-2,3,1),  new Vector3(4,0,0),  new Vector3(0,0,4),  3));
        scene.Objects.Add(new Quad(new Vector3(-2,-3,5),  new Vector3(4,0,0),  new Vector3(0,0,-4),  4));
        return scene;
    }
    
    
    public static Scene Earth()
    {
        var scene = new Scene();
        var image = new ImageTexture(@".\assets\textures\earthmap.jpg");
        scene.Materials.Add(new Lambertian(image));
        scene.Objects.Add(new Sphere(Vector3.Zero, 2, 0));
        return scene;
    }

    public static Scene Book1Cover()
    {
        var scene = new Scene();
        //textures
        var checker = new CheckerTexture(0.32f, new Vector3(.2f, .3f, .1f), new Vector3(.9f, .9f, .9f));
        //main elements
        scene.Materials.Add(new Lambertian(checker));
        scene.Materials.Add(new Dielectric {IoR = 1.5f});
        scene.Materials.Add(new Lambertian(0.4f, 0.2f, 0.1f));
        scene.Materials.Add(new Metal {Albedo = new Vector3(0.7f, 0.6f, 0.5f), Roughness = 0.0f});
        scene.Objects.Add(new Sphere (new Vector3(0, -1000, 0), 1000f,  0));
        scene.Objects.Add(new Sphere(new Vector3(0, 1, 0), 1.0f, 1));
        scene.Objects.Add(new Sphere (new Vector3(-4, 1, 0), 1.0f,  2));
        scene.Objects.Add(new Sphere (new Vector3(4, 1, 0), 1.0f,  3));
        //random elements
        var index = 4; //as 4 materials are created already!
        var seed = (uint) DateTime.Now.Microsecond;
        for (var a = -11; a < 11; a++)
        {
            for (var b = -11; b < 11; b++)
            {
                var randomMaterialSelector = Random.Float(ref seed);
                var center = new Vector3(a + 0.9f * Random.Float(ref seed), 0.2f, b + 0.9f * Random.Float(ref seed));
                if ((center - new Vector3(4, .3f, 0)).Length() > .9f)
                {
                    if (randomMaterialSelector < .8f)
                    {
                        var albedo = Random.Vec3(ref seed, 0, 1) * Random.Vec3(ref seed, 0, 1);
                        scene.Materials.Add(new Lambertian(albedo));
                    }
                    else if (randomMaterialSelector < .95f)
                    {
                        var albedo = Random.Vec3(ref seed, 0.5f, 1);
                        var roughness = Random.Float(ref seed, 0, 1);
                        scene.Materials.Add(new Metal {Albedo = albedo, Roughness = roughness});
                    }
                    else
                        scene.Materials.Add(new Dielectric {IoR = 1.5f});

                    scene.Objects.Add(new Sphere (center, .2f,  index));
                    index++;
                }
            }
        }

        return scene;
    }
}
