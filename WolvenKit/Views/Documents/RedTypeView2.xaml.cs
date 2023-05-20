#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.TreeGrid;
using WolvenKit.App.Helpers;
using WolvenKit.App.ViewModels.Shell.RedTypes;
using WolvenKit.RED4.Types;
using YamlDotNet.Core.Tokens;
using IRedType = WolvenKit.RED4.Types.IRedType;

namespace WolvenKit.Views.Documents
{
    /// <summary>
    /// Interaktionslogik für RedTypeView2.xaml
    /// </summary>
    public partial class RedTypeView2 : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(IRedType), typeof(RedTypeView2), new PropertyMetadata(null, OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RedTypeView2 view)
            {
                return;
            }

            view.RefreshData();
        }

        public IRedType? Source
        {
            get => (IRedType?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private PropertyViewModel? _test;

        public ObservableCollection<PropertyViewModel> Nodes { get; } = new();

        public RedTypeView2() => InitializeComponent();

        private void TreeGrid_OnNodeExpanded(object? sender, NodeExpandedEventArgs e)
        {
            if (e.Node.ChildNodes.Count == 1)
            {
                TreeGrid.ExpandNode(e.Node.ChildNodes[0]);
            }
        }

        private void RefreshData()
        {
            Nodes.Clear();

            if (Source == null)
            {
                return;
            }

            _test = PropertyViewModel.Create(Source);
            foreach (var viewModel in _test.DisplayCollection)
            {
                Nodes.Add(viewModel);
            }
        }
    }
}
