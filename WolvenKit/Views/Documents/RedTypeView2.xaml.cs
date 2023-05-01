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
using WolvenKit.RED4.Types;

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

        public ObservableCollection<Node2> Nodes { get; } = new();

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

            GenerateData(Nodes, Source);
        }

        public static bool IsHiddenProperty(Type type, string name)
        {
            if (type.IsAssignableTo(typeof(scnSceneGraphNode)) && name is "NodeId" or "OutputSockets")
            {
                return true;
            }

            if (type.IsAssignableTo(typeof(scnQuestNode)) && name is "IsockMappings" or "OsockMappings")
            {
                return true;
            }

            if (type.IsAssignableTo(typeof(graphGraphNodeDefinition)) && name is "Sockets")
            {
                return true;
            }

            if (type.IsAssignableTo(typeof(questNodeDefinition)) && name is "Id")
            {
                return true;
            }

            return false;
        }

        public static void GenerateData(ICollection<Node2> list, IRedType? data)
        {
            if (data == null)
            {
                return;
            }

            var dataType = data.GetType();
            foreach (var propertyInfo in dataType.GetProperties())
            {
                if (IsHiddenProperty(dataType, propertyInfo.Name))
                {
                    continue;
                }

                var childItem = new Node2(data, propertyInfo);
                if (childItem.Value is IRedBaseHandle handle)
                {
                    GenerateData(childItem.Properties, handle.GetValue());
                }

                if (childItem.Value is IRedArray array)
                {
                    var childItemProperty = childItem.Value.GetType().GetProperty("Item")!;

                    childItem.DisplayValue = $"Count = {array.Count}";
                    for (var i = 0; i < array.Count; i++)
                    {
                        var tmp = new Node2(childItem.Value, childItemProperty, i) {Name = i.ToString()};
                        GenerateData(tmp.Properties, (IRedType?)array[i]);
                        childItem.Properties.Add(tmp);
                    }
                }

                if (childItem.Value is RedBaseClass child)
                {
                    GenerateData(childItem.Properties, child);
                }

                if (childItem.Value is IRedRef resourcePath)
                {
                    //childItem.Value = resourcePath.DepotPath.GetResolvedText();
                }

                list.Add(childItem);
            }
        }

        public class Node
        {
            public Node(string name, object? value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public object? Value { get; set; }
            public ObservableCollection<Node> Properties { get; } = new();
        }

        public class Node2 : INotifyPropertyChanged
        {
            private readonly object _parent;
            private readonly PropertyInfo _propertyInfo;
            private readonly int _index;

            private string _displayValue;

            public Node2(object parent, PropertyInfo propertyInfo, int index = -1)
            {
                _parent = parent;
                _propertyInfo = propertyInfo;
                _index = index;

                Name = propertyInfo.Name;
                _displayValue = Value?.ToString() ?? "null";
            }

            public string Name { get; set; }

            public Type Type => _propertyInfo.PropertyType;

            public string DisplayValue
            {
                get => _displayValue;
                set
                {
                    _displayValue = value;
                    OnPropertyChanged();
                }
            }

            public object? Value
            {
                get => _propertyInfo.GetValue(_parent, _index > -1 ? new object[] { _index } : null);
                set => SetValue(value);
            }

            public ObservableCollection<Node2> Properties { get; } = new();

            private void SetValue(object? value)
            {
                _propertyInfo.SetValue(_parent, value, _index > -1 ? new object[] { _index } : null);
                OnPropertyChanged();

                if (value is IRedHandle handle)
                {
                    Properties.Clear();
                    GenerateData(Properties, handle.GetValue());
                }
                
                DisplayValue = Value?.ToString() ?? "null";
            }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            #endregion INotifyPropertyChanged
        }
    }
}
