using System.Numerics;
using ImGuiNET;
using Licht.Applications;
using Licht.Vulkan;
using Microsoft.Extensions.Logging;
using Pathtracer.RenderObjects;
using Silk.NET.Windowing;

namespace Pathtracer;

public class PathtracingApplication : ImGuiApplication
{
    
    private readonly Diagnoser _diagnoser = new();
    private readonly Pathtracer _pathtracer;
    
    private uint _viewportWidth;
    private uint _viewportHeight;
    private bool _stop;

    private readonly Scene _scene = Scene.CornellBox();

    private Camera _camera = new()
    // {
    //     VerticalFovDegrees = 40,
    //     Position = new Vector3(478, 278, -600),
    //     LookAt = new Vector3(278, 278, 0)
    // };
    {
        VerticalFovDegrees = 40,
        Position = new Vector3(278, 278, -800),
        LookAt = new Vector3(278, 278, 0)
    };
    // {
    //     VerticalFovDegrees = 80,
    //     Position = new Vector3(0, 0,9),
    //     LookAt = Vector3.Zero,
    //     DepthOfFieldSettings = new DepthOfFieldSettings(0,100)
    // };
    // {
    //     VerticalFovDegrees = 20,
    //     Position = new Vector3(13,2,3),
    //     LookAt = new Vector3(0,0,0),
    //     Up = new Vector3(0,1,0),
    //     DepthOfFieldSettings = new DepthOfFieldSettings(0.6f, 10.0f)
    // };

    public PathtracingApplication(ILogger logger, VkRenderer renderer, IWindow window) : base(logger, renderer, window)
    {
        _pathtracer = new Pathtracer(renderer.Device, UiContext);
        PushStyle();
        _pathtracer.LoadScene(_scene);
    }
    private void PushStyle()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6,6));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6,4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding, 4);
    }

    public override void DrawUI(CommandBuffer cmd, float deltaTime)
    {
        base.DrawUI(cmd, deltaTime);
        ImGui.DockSpaceOverViewport();
        ImGui.Begin("Information");
        ImGui.Text($"Last Render: {_diagnoser.LastRenderTime:N3} ms");
        ImGui.Text($"Total Time: {_diagnoser.TotalRenderTime / 1000:N3}s");
        ImGui.Text($"Frames: {_pathtracer.FrameIndex}");
        ImGui.Separator();
        if(ImGui.Button("Save Image")) _pathtracer.SaveImageToDisk();
        ImGui.End();

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.Begin("Viewport");
        _viewportWidth = (uint)ImGui.GetContentRegionAvail().X;
        _viewportHeight = (uint)ImGui.GetContentRegionAvail().Y;
        if (_pathtracer.FinalImage is not null)
            ImGui.Image((nint) _pathtracer.FinalImage.ImGuiDescriptorSet.Handle,
                new Vector2(_viewportWidth, _viewportHeight));
        ImGui.End();
        ImGui.PopStyleVar();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        _diagnoser.BeginFrame();
        
        _camera.OnResize(_viewportWidth, _viewportHeight);
        if(_pathtracer.OnResize(_viewportWidth, _viewportHeight)) _diagnoser.Reset();
        _pathtracer.OnRender(_camera);
        
        _diagnoser.EndFrame();
    }

    public override void Release()
    {
        base.Release();
        _diagnoser.Reset();
        _pathtracer.Dispose();
    }
}
