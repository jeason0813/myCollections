using System;
using System.Collections;

namespace myCollections.Controls
{
    public interface IProgressOperation
    {
        /// <summary>
        /// Gets the progress total (the number that represents the progress end)
        /// </summary>
        int Total { get; }
        int AddedItem { get; }
        int NotAddedItem { get; }
        int Current { get; }

        string Message { get; }

        IList RemovedItems { get; }

        /// <summary>
        /// Starts the progress operation
        /// </summary>
        void Start();

        /// <summary>
        /// Requests cancellation of the progress operation
        /// </summary>
        void CancelAsync();

        /// <summary>
        /// Occurs when the progress of the operation has changed
        /// </summary>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Occurs when the total of the progress operation has changed (the number that represents the progress end)
        /// </summary>
        event EventHandler ProgressTotalChanged;

        /// <summary>
        /// Occurs when the progress operation is complete
        /// </summary>
        event EventHandler Complete;

        event EventHandler MessageChanged;
    }
}

