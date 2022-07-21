using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorCanvas;

/// <summary>
/// Represents the base rendering context for a canvas.
/// </summary>
public abstract class RenderingContext : IDisposable
{
    #region Constants

    private const string NAMESPACE_PREFIX = "BlazorExtensions";
    private const string GET_PROPERTY_ACTION = "getProperty";
    private const string CALL_METHOD_ACTION = "call";
    private const string CALL_BATCH_ACTION = "callBatch";
    private const string ADD_ACTION = "add";
    private const string REMOVE_ACTION = "remove";

    #endregion

    private readonly List<object[]> batchedCallObjects = new List<object[]>();
    private readonly string contextName;
    private readonly IJSRuntime jsRuntime;
    private readonly object parameters;
    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    private bool awaitingBatchedCall;
    private bool batching;
    private bool initialized;

    /// <summary>
    /// Initializes <see cref="RenderingContext"/>.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="contextName"></param>
    /// <param name="parameters"></param>
    internal RenderingContext(BECanvas reference, string contextName, object parameters = null)
    {
        this.Canvas = reference.CanvasReference;
        this.contextName = contextName;
        this.jsRuntime = reference.JSRuntime;
        this.parameters = parameters;
    }

    public ElementReference Canvas { get; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously; Reason: extension point for subclasses, which may do asynchronous work
    protected virtual async Task ExtendedInitializeAsync() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal async Task<RenderingContext> InitializeAsync()
    {
        await this.semaphoreSlim.WaitAsync();

        if (!this.initialized)
        {
            await this.jsRuntime.InvokeAsync<object>($"{NAMESPACE_PREFIX}.{this.contextName}.{ADD_ACTION}", this.Canvas, this.parameters);
            await this.ExtendedInitializeAsync();
            this.initialized = true;
        }

        this.semaphoreSlim.Release();

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task BeginBatchAsync()
    {
        await this.semaphoreSlim.WaitAsync();
        this.batching = true;
        this.semaphoreSlim.Release();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task EndBatchAsync()
    {
        await this.semaphoreSlim.WaitAsync();

        await this.BatchCallInnerAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isMethodCall"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected async Task BatchCallAsync(string name, bool isMethodCall, params object[] value)
    {
        await this.semaphoreSlim.WaitAsync();

        var callObject = new object[value.Length + 2];
        callObject[0] = name;
        callObject[1] = isMethodCall;
        Array.Copy(value, 0, callObject, 2, value.Length);
        this.batchedCallObjects.Add(callObject);

        if (this.batching || this.awaitingBatchedCall)
        {
            this.semaphoreSlim.Release();
        }
        else
        {
            await this.BatchCallInnerAsync();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    protected async Task<T> GetPropertyAsync<T>(string property)
        => await this.jsRuntime.InvokeAsync<T>(
            $"{NAMESPACE_PREFIX}.{this.contextName}.{GET_PROPERTY_ACTION}",
            this.Canvas, property);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    protected T CallMethod<T>(string method)
        => this.CallMethodAsync<T>(method)
               .GetAwaiter()
               .GetResult();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    protected async Task<T> CallMethodAsync<T>(string method)
        => await this.jsRuntime.InvokeAsync<T>(
            $"{NAMESPACE_PREFIX}.{this.contextName}.{CALL_METHOD_ACTION}",
            this.Canvas, method);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected T CallMethod<T>(string method, params object[] value)
        => this.CallMethodAsync<T>(method, value)
               .GetAwaiter()
               .GetResult();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected async Task<T> CallMethodAsync<T>(string method, params object[] value)
        => await this.jsRuntime.InvokeAsync<T>(
            $"{NAMESPACE_PREFIX}.{this.contextName}.{CALL_METHOD_ACTION}",
            this.Canvas, method, value);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task BatchCallInnerAsync()
    {
        this.awaitingBatchedCall = true;
        var currentBatch = this.batchedCallObjects.ToArray();
        this.batchedCallObjects.Clear();
        this.semaphoreSlim.Release();

        _ = await this.jsRuntime.InvokeAsync<object>($"{NAMESPACE_PREFIX}.{this.contextName}.{CALL_BATCH_ACTION}", this.Canvas, currentBatch);

        await this.semaphoreSlim.WaitAsync();
        this.awaitingBatchedCall = false;
        this.batching = false;
        this.semaphoreSlim.Release();
    }

    /// <summary>
    /// Disposes the rendering context.
    /// </summary>
    public void Dispose()
        => Task.Run(async ()
            => await this.jsRuntime.InvokeAsync<object>(
                $"{NAMESPACE_PREFIX}.{this.contextName}.{REMOVE_ACTION}",
                this.Canvas));
}
