using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace DaFiles.Helpers.UI;

public class SetGlobalPropertiesAction : StyledElementAction
{
    public TextRenderingMode TextRenderingMode { get; init; }

    public override object? Execute(object? sender, object? parameter)
    {
        if (sender is TopLevel topLevel)
        {
            RenderOptions.SetTextRenderingMode(topLevel, TextRenderingMode);
        }
        return null;
    }
}
