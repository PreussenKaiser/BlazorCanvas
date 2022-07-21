using Blazor.Extensions.Canvas.Canvas2D;
using BlazorCanvas.Extensions;

namespace BlazorCanvas.Test.Client.Pages
{
    /// <summary>
    /// Code behind for the Index.razor component.
    /// </summary>
    public partial class Index
    {
        /// <summary>
        /// API wrapper.
        /// </summary>
        private Canvas2DContext context;

        /// <summary>
        /// The reference to the canvas element.
        /// </summary>
        protected BECanvas canvas;

        /// <summary>
        /// Initializes the canvas.
        /// </summary>
        /// <param name="firstRender">Whether this is the first render or not.</param>
        /// <returns>Whether the task was completed or not.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            this.context = await this.canvas.CreateCanvas2DAsync();

            await this.context.SetFillStyleAsync("green");
            await this.context.SetFontAsync("48px serif");

            await this.context.FillRectAsync(10, 100, 100, 100);
            await this.context.StrokeTextAsync("Hello Blazor!!!", 10, 100);
        }
    }
}