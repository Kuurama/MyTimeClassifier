using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MyTimeClassifier.ViewModels;
using System;

namespace MyTimeClassifier.Utils;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? p_Data)
    {
        if (p_Data is null)
            return null;

        var l_Name = p_Data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var l_Type = Type.GetType(l_Name);

        if (l_Type == null) return new TextBlock { Text = "Not Found: " + l_Name };

        var l_Control = (Control)Activator.CreateInstance(l_Type)!;
        l_Control.DataContext = p_Data;
        return l_Control;
    }

    public bool Match(object? p_Data) => p_Data is ViewModelBase;
}
