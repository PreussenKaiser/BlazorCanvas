using Blazor.Extensions.Canvas.Canvas2D;
using BlazorCanvas.Extensions;

using Microsoft.AspNetCore.Components.Web;
using System.Numerics;

namespace BlazorCanvas.Test.Client.Pages;

/// <summary>
/// Code-behind for Drawing.razor.
/// </summary>
public partial class Drawing
{
    /// <summary>
    /// A reference to the canvas component.
    /// </summary>
    private BECanvas canvas;

    /// <summary>
    /// API wrapper for the canvas.
    /// </summary>
    private Canvas2DContext context;

    /// <summary>
    /// The current drawing position.
    /// </summary>
    private Vector2 position;

    /// <summary>
    /// Creates the canvas context.
    /// </summary>
    /// <param name="firstRender">Whether this was the first render or not.</param>
    /// <returns>Whether the task was completed or not.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            this.context = await this.canvas.CreateCanvas2DAsync();
    }

    /// <summary>
    /// Draws on the canvas.
    /// </summary>
    /// <param name="args">Pointer arguments.</param>
    /// <returns>Whether the task was completed or not.</returns>
    private async Task DrawAsync(MouseEventArgs args)
    {
        // Make sure it's the left mouse click.
        if (args.Buttons != 1)
            return;

        await this.context.BeginPathAsync();

        await this.context.SetLineWidthAsync(5);
        await this.context.SetLineCapAsync(LineCap.Round);
        await this.context.SetStrokeStyleAsync("#c0392b");

        await this.context.MoveToAsync(
            this.position.X,
            this.position.Y);

        this.SetPosition(args);

        await this.context.LineToAsync(
            this.position.X,
            this.position.Y);

        await this.context.StrokeAsync();
    }

    /// <summary>
    /// Sets the current drawing position.
    /// </summary>
    /// <param name="args">Pointer arguments.</param>
    private void SetPosition(MouseEventArgs args)
    {
        // TODO: Find a better way to center the pointer.
        this.position.X = (float)args.ClientX - 285;
        this.position.Y = (float)args.ClientY - 135;
    }
}
