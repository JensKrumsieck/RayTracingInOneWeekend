using System.Diagnostics;

namespace Pathtracer;

public class Diagnoser
{
    public double LastRenderTime { get; private set; }
    public double TotalRenderTime { get; private set; }
    
    private readonly Stopwatch _frameStopwatch = new();
    private readonly Stopwatch _renderStopwatch = new();
    private bool _begun;

    public void BeginFrame()
    {
        _frameStopwatch.Start();
        if (_begun) return;
        _renderStopwatch.Start();
        _begun = true;
    }

    public void EndFrame()
    {
        _frameStopwatch.Stop();
        LastRenderTime = _frameStopwatch.Elapsed.TotalMilliseconds;
        TotalRenderTime = _renderStopwatch.Elapsed.TotalMilliseconds;
        _frameStopwatch.Reset();
    }

    public void Reset()
    {
        _renderStopwatch.Stop();
        _renderStopwatch.Reset();
        _begun = false;
        TotalRenderTime = 0;
    }
}
