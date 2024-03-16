using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotUtilities.Core.Abstraction
{
    /// <summary>
    /// Represents an asynchronously disposable resource. Implements <see cref="IAsyncDisposable"/> for
    /// asynchronous resource cleanup.
    /// </summary>
    public partial class AsyncDisposableResource : IAsyncDisposable
    {
        /// <summary>
        /// Indicates if the object has been disposed. True if resources have been released, false otherwise.
        /// </summary>
        protected internal bool disposed = false;

        /// <summary>
        /// Finalizer that ensures the asynchronous disposal of resources. 
        /// </summary>
        ~AsyncDisposableResource()
        {
            // Finalizer indirectly invokes DisposeAsync to release resources.
            _ = DisposeAsync(disposing: false);
        }

        /// <summary>
        /// Asynchronously disposes managed resources. This method is called upon object disposal and may be overridden in derived classes
        /// for custom asynchronous disposal logic.
        /// </summary>
        /// <param name="disposing">Indicates whether the method has been called directly or indirectly by a user's code.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous disposal operation.</returns>
        public virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!disposed)
            {
                // Perform async cleanup for managed resources here.
                if (disposing)
                {
                    await ReleaseManagedResourcesAsync();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            // Suppress finalization to prevent the finalizer from running again.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronously releases managed resources. Override this method in derived classes to release specific
        /// managed resources asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation.</returns>
        protected virtual async Task ReleaseManagedResourcesAsync()
        {
            // Resource release logic for derived classes. Completes immediately if no resources to release.
            if (!disposed)
                await Task.CompletedTask;
        }
    }
}
