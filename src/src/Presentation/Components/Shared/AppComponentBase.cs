using Microsoft.AspNetCore.Components;

namespace Presentation.Components.Shared;

/// <summary>
/// The base component for all generic apps in the project.
/// Has a protected CancellationToken
/// </summary>
public abstract class AppComponentBase : ComponentBase, IDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// The cancellation token for the component.
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