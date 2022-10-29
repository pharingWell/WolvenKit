using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
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
using Splat;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.ViewModels.Tools;

namespace WolvenKit.Views.Tools;
/// <summary>
/// Interaktionslogik für SaveGameBrowserView.xaml
/// </summary>
public partial class SaveGameBrowserView
{
    public SaveGameBrowserView()
    {
        InitializeComponent();

        ViewModel = Locator.Current.GetService<SaveGameBrowserViewModel>();

        this.WhenActivated((disposables) =>
        {
            ViewModel.SetupSaveGames();
        });
    }
}
