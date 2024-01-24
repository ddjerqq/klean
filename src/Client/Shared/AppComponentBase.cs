using Microsoft.AspNetCore.Components;

namespace Client.Shared;

/// <inheritdoc cref="Microsoft.AspNetCore.Components.ComponentBase" />
public abstract class AppComponentBase : ComponentBase, IDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken => (_cancellationTokenSource ??= new CancellationTokenSource()).Token;

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_cancellationTokenSource is null)
            return;

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }
}