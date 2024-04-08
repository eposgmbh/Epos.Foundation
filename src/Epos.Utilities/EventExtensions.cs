using System;
using System.ComponentModel;

namespace Epos.Utilities;

/// <summary>Collection of extension methods for event raising.</summary>
public static class EventExtensions
{
    /// <summary>Raises an <see cref="System.EventHandler"/> event.</summary>
    /// <param name="handler">Event handler</param>
    /// <param name="sender">Sender</param>
    public static void Raise(this EventHandler? handler, object sender) => handler.Raise(sender, EventArgs.Empty);

    /// <summary>Raises an <see cref="System.EventHandler"/> event with the specified
    /// <see cref="System.EventArgs"/>.</summary>
    /// <param name="handler">Event handler</param>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args (if <b>null</b>, <see cref="EventArgs.Empty"/> is used)</param>
    public static void Raise(this EventHandler? handler, object sender, EventArgs e) =>
        handler?.Invoke(sender, e ?? EventArgs.Empty);

    /// <summary>Raises an <see cref="System.EventHandler{TEventArgs}"/> event with the specified
    /// <typeparamref name="TEventArgs"/>.</summary>
    /// <param name="handler">Event handler</param>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    public static void Raise<TEventArgs>(
        this EventHandler<TEventArgs>? handler, object sender, TEventArgs e
    ) where TEventArgs : EventArgs {
        if (e is null) {
            throw new ArgumentNullException(nameof(e));
        }

        handler?.Invoke(sender, e);
    }

    /// <summary>Raises a <b>PropertyChangedEventHandler</b> event with the
    /// specified <paramref name="propertyName"/>.</summary>
    /// <param name="handler">Event handler</param>
    /// <param name="sender">Sender</param>
    /// <param name="propertyName">Property name (<b>null</b> means that all of
    /// the properties have changed)</param>
    public static void Raise(this PropertyChangedEventHandler? handler, object sender, string propertyName) =>
        handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
}
