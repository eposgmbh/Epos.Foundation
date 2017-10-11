using System;
using System.ComponentModel;

namespace Epos.Utilities
{
    public static class EventExtensions
    {
        public static void Raise(this EventHandler handler, object sender) {
            handler.Raise(sender, EventArgs.Empty);
        }

        public static void Raise(this EventHandler handler, object sender, EventArgs e) {
            handler?.Invoke(sender, e ?? EventArgs.Empty);
        }

        public static void Raise<TEventArgs>(
            this EventHandler<TEventArgs> handler, object sender, TEventArgs e
        ) where TEventArgs : EventArgs {
            handler?.Invoke(sender, e);
        }

        public static void Raise(this PropertyChangedEventHandler handler, object sender, string propertyName) {
            handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}