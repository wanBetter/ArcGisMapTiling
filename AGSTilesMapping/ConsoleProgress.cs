using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:System.IProgress`1" /> for console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ConsoleProgress<T> : IProgress<T>
    {
        private readonly Action<T> _action;

        public ConsoleProgress(Action<T> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Report(T value) => _action(value);
    }
}
