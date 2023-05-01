using System.Windows;
using System.Windows.Controls;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Documents;

namespace WolvenKit.Views.Nodes;

public class RedTypeCellTemplateSelector : RedTypeEditTemplateSelector
{
    public DataTemplate Done { get; set; }
    public DataTemplate NotDone { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not RedTypeView2.Node2 node)
        {
            return ReadOnly;
        }

        var value = node.Value;
        if (value is RedBaseClass or IRedHandle or IRedArray)
        {
            return ReadOnly;
        }

        if (value is CBool)
        {
            return CBool;
        }

        if (value is CName)
        {
            return ReadOnly;
        }

        if (value is CString)
        {
            return ReadOnly;
        }

        if (value is NodeRef)
        {
            return ReadOnly;
        }

        if (value is CInt32)
        {
            return ReadOnly;
        }

        if (value is IRedEnum)
        {
            return ReadOnly;
        }

        return NotDone;
    }
}

public class RedTypeEditTemplateSelector : DataTemplateSelector
{
    public DataTemplate ReadOnly { get; set; }
    public DataTemplate CBool { get; set; }
    public DataTemplate CName { get; set; }
    public DataTemplate CString { get; set; }
    public DataTemplate CInt32 { get; set; }
    public DataTemplate NodeRef { get; set; }
    public DataTemplate CEnum { get; set; }
    public DataTemplate CHandle { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not RedTypeView2.Node2 node)
        {
            return ReadOnly;
        }

        var value = node.Value;
        if (value is CBool)
        {
            return CBool;
        }

        if (value is CName)
        {
            return CName;
        }

        if (value is CString)
        {
            return CString;
        }

        if (value is NodeRef)
        {
            return NodeRef;
        }

        if (value is CInt32)
        {
            return CInt32;
        }

        if (value is IRedEnum)
        {
            return CEnum;
        }

        if (value is IRedHandle)
        {
            return CHandle;
        }

        return ReadOnly;
    }
}