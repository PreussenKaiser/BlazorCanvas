using Blazor.Extensions.Canvas.Canvas2D;
using BlazorCanvas.WebGL;

namespace BlazorCanvas.Extensions;

public static class CanvasContextExtensions
{
    public static Canvas2DContext CreateCanvas2D(this BECanvas canvas)
    {
        return new Canvas2DContext(canvas)
            .InitializeAsync()
            .GetAwaiter()
            .GetResult() as Canvas2DContext;
    }

    public static async Task<Canvas2DContext> CreateCanvas2DAsync(this BECanvas canvas)
    {
        return await new Canvas2DContext(canvas)
            .InitializeAsync()
            .ConfigureAwait(false) as Canvas2DContext;
    }

    public static WebGLContext CreateWebGL(this BECanvas canvas)
    {
        return new WebGLContext(canvas)
            .InitializeAsync()
            .GetAwaiter()
            .GetResult() as WebGLContext;
    }

    public static async Task<WebGLContext> CreateWebGLAsync(this BECanvas canvas)
    {
        return await new WebGLContext(canvas)
            .InitializeAsync()
            .ConfigureAwait(false) as WebGLContext;
    }

    public static WebGLContext CreateWebGL(this BECanvas canvas, WebGLContextAttributes attributes)
    {
        return new WebGLContext(canvas, attributes)
            .InitializeAsync()
            .GetAwaiter()
            .GetResult() as WebGLContext;
    }

    public static async Task<WebGLContext> CreateWebGLAsync(this BECanvas canvas, WebGLContextAttributes attributes)
    {
        return await new WebGLContext(canvas, attributes)
            .InitializeAsync()
            .ConfigureAwait(false) as WebGLContext;
    }
}
