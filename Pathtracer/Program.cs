using Licht.Applications;
using Licht.Core;
using Licht.Vulkan.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pathtracer;

var opts = ApplicationSpecification.Default with {ApplicationName = "Triangle"};
var builder = new ApplicationBuilder(opts);

builder.Services.AddSingleton<ILogger, Logger>();
builder.Services.AddWindow(opts);
builder.Services.AddVulkanRenderer<PassthroughAllocator>();

{
    using var app = builder.Build<PathtracingApplication>();
    app.Run();
}
