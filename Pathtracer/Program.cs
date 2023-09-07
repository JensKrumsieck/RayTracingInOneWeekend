using Catalyze;
using Catalyze.Allocation;
using Catalyze.Applications;
using Pathtracer;
using Silk.NET.Input.Glfw;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

GlfwWindowing.RegisterPlatform();;
GlfwInput.RegisterPlatform();
GlfwWindowing.Use();

var builder = Application.CreateBuilder();
var window = builder.Services.AddWindowing(WindowOptions.DefaultVulkan);
builder.Services.AddInput(window);
builder.Services.RegisterSingleton<IAllocator, PassthroughAllocator>();

var app = builder.Build();

var options = new GraphicsDeviceCreateOptions();

app.UseVulkan(options)
    .UseImGui()
    .AttachLayer<AppLayer>();
app.Run();

app.Dispose();