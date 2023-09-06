using System.Numerics;
using Random = Catalyze.Random;

namespace Pathtracer;

public class Scene
{
    public readonly List<Shape> Objects = new();
    public readonly List<Material> Materials = new();

    public static Scene Book1Cover()
    {
        var scene = new Scene();
        //main elements
        scene.Materials.Add(new Lambertian {Albedo = new Vector3(.5f, .5f, .5f)});
        scene.Materials.Add(new Dielectric {IoR = 1.5f});
        scene.Materials.Add(new Lambertian {Albedo = new Vector3(0.4f, 0.2f, 0.1f)});
        scene.Materials.Add(new Metal {Albedo = new Vector3(0.7f, 0.6f, 0.5f), Roughness = 0.0f});
        scene.Objects.Add(new Sphere {Position = new Vector3(0, -1000, 0), Radius = 1000f, MaterialIndex = 0});
        scene.Objects.Add(new Sphere {Position = new Vector3(0, 1, 0), Radius = 1.0f, MaterialIndex = 1});
        scene.Objects.Add(new Sphere {Position = new Vector3(-4, 1, 0), Radius = 1.0f, MaterialIndex = 2});
        scene.Objects.Add(new Sphere {Position = new Vector3(4, 1, 0), Radius = 1.0f, MaterialIndex = 3});
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
                        scene.Materials.Add(new Lambertian {Albedo = albedo});
                    }
                    else if (randomMaterialSelector < .95f)
                    {
                        var albedo = Random.Vec3(ref seed, 0.5f, 1);
                        var roughness = Random.Float(ref seed, 0, 1);
                        scene.Materials.Add(new Metal {Albedo = albedo, Roughness = roughness});
                    }
                    else
                        scene.Materials.Add(new Dielectric {IoR = 1.5f});

                    scene.Objects.Add(new Sphere {Position = center, Radius = .2f, MaterialIndex = index});
                    index++;
                }
            }
        }

        return scene;
    }
}
