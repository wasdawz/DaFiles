using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System;

namespace DaFiles.Helpers;

/// <summary>
/// A navigation page factory that resolves pages using a specified method.
/// </summary>
/// <param name="getPageFromObject">Method to use for <see cref="INavigationPageFactory.GetPageFromObject(object)"/>.</param>
internal class CustomNavigationPageFactory(Func<object, Control> getPageFromObject) : INavigationPageFactory
{
    private readonly Func<object, Control> _getPageFromObject = getPageFromObject;

    public Control GetPage(Type srcType)
    {
        throw new InvalidOperationException();
    }

    public Control GetPageFromObject(object target) => _getPageFromObject(target);
}
