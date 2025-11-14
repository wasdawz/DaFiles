using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace DaFiles.Helpers;

internal static class ExtensionMethods
{
    /// <summary>
    /// If <paramref name="visual"/> has a root <see cref="Window"/>, shows the <paramref name="window"/>
    /// as a child of that window; otherwise, shows the <paramref name="window"/> without an owner.
    /// </summary>
    /// <param name="visual">Element that may have a root <see cref="Window"/> that should be used
    /// as an owner of the <paramref name="window"/>.</param>
    public static void ShowTryingAsChild(this Window window, Visual visual)
    {
        if (TopLevel.GetTopLevel(visual) is Window ownerWindow)
            window.Show(ownerWindow);
        else
            window.Show();
    }

    public static string? Read(this SecureString secureString)
    {
        nint stringPtr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
        string? str = Marshal.PtrToStringUni(stringPtr);
        Marshal.ZeroFreeCoTaskMemUnicode(stringPtr);
        return str;
    }

    /// <summary>
    /// Projects each nullable element of an observable sequence into a new observable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the inner sequences, obtained by running the selector function for each element in the source sequence.</typeparam>
    /// <param name="source">A sequence of elements to invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to non-<see langword="null"/> source elements.</param>
    /// <returns><para>An observable sequence whose elements are either:</para>
    /// <para>- A sequence created by invoking the transform function on an element, if source element is non-<see langword="null"/>;</para>
    /// <para>- An empty sequence whose observers will never get called, if source element is <see langword="null"/>.</para></returns>
    public static IObservable<IObservable<TResult>> SelectInnerSequenceNullable<TSource, TResult>(this IObservable<TSource?> source, Func<TSource, IObservable<TResult>> selector)
    {
        return source.Select(el => el is null ? Observable.Never<TResult>(default!) : selector(el));
    }

    /// <summary>
    /// Gets the value associated with the specified key. If the value doesn't exist, creates and adds it to the dictionary.
    /// </summary>
    /// <param name="key">The key of the value to get or create.</param>
    /// <param name="factory">Method used to create the value if it doesn't exist.</param>
    /// <returns>Already existing or newly created value associated with the specified key.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory) where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out TValue? value))
            dictionary.Add(key, value = factory());
        return value;
    }
}
