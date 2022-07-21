using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorCanvas;

/// <summary>
/// The canvas API wrapper component.
/// </summary>
public partial class BECanvas
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BECanvas"/> class.
    /// </summary>
    public BECanvas()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CssClass = string.Empty;
    }

    /// <summary>
    /// Gets or sets the canvas' height.
    /// </summary>
    [Parameter]
    public long Height { get; set; }

    /// <summary>
    /// Gets or sets the canvas' width.
    /// </summary>
    [Parameter]
    public long Width { get; set; }

    /// <summary>
    /// Gets or sets the canvas' unique identifier.
    /// </summary>
    [Parameter]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the canvas' CSS class.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the event to call when the mouse moves.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnMouseMove { get; set; }

    /// <summary>
    /// Gets or sets the event to call when the mouse hovers over the canvas.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnMouseOver { get; set; }

    /// <summary>
    /// A reference to the component.
    /// </summary>
    private ElementReference canvasRef;

    /// <summary>
    /// Gets a reference to the component.
    /// </summary>
    public ElementReference CanvasReference
        => this.canvasRef;

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject]
    internal IJSRuntime JSRuntime { get; set; }
}
