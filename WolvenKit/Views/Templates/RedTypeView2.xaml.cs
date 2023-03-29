using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Syncfusion.Data;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Helpers;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Tools;

namespace WolvenKit.Views.Editors;
/// <summary>
/// Interaktionslogik für RedTypeView2.xaml
/// </summary>
public partial class RedTypeView2
{
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(PropertyViewModel), typeof(RedTypeView2), new PropertyMetadata(OnSelectedItemChanged));

    public PropertyViewModel SelectedItem
    {
        get => (PropertyViewModel)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public RedTypeView2()
    {
        InitializeComponent();

        RedDataGrid.SortComparers.Add(new SortComparer() { Comparer = new PropertyViewComparer(), PropertyName = "DisplayName" });
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RedTypeView2 view)
        {
            return;
        }
    }
}

public class RedViewModelViewHost : ViewModelViewHost
{
    public RedViewModelViewHost() => ViewLocator = new RedViewLocator();

    private class RedViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null)
        {
            if (viewModel is DefaultPropertyViewModel propertyViewModel)
            {
                if (propertyViewModel.RedPropertyInfo.BaseType.IsAssignableTo(typeof(CName)))
                {
                    return new RedCNameEditor();
                }
                if (propertyViewModel.RedPropertyInfo.BaseType.IsAssignableTo(typeof(CFloat)))
                {
                    return new RedFloatEditor();
                }
                if (propertyViewModel.RedPropertyInfo.BaseType.IsAssignableTo(typeof(CUInt64)))
                {
                    return new RedUlongEditor();
                }
                if (propertyViewModel.RedPropertyInfo.BaseType.IsAssignableTo(typeof(IRedInteger)))
                {
                    return new RedIntegerEditor();
                }
            }

            return null;
        }
    }
}
