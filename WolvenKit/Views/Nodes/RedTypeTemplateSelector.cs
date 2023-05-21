using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.ViewModels.Shell.RedTypes;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Documents;

namespace WolvenKit.Views.Nodes;

public class RedTypeCellTemplateSelector : RedTypeEditTemplateSelector
{
    public DataTemplate Done { get; set; }
    public DataTemplate NotDone { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not PropertyViewModel node)
        {
            return ReadOnly;
        }

        var value = node.DataObject;
        if (value is RedBaseClass or IRedHandle or IRedBaseArray)
        {
            return ReadOnly;
        }

        if (value is CBool)
        {
            return CBool;
        }

        if (value is IRedRef)
        {
            return ResourcePath;
        }

        if (value is CName)
        {
            return ReadOnly;
        }

        if (value is CString)
        {
            return ReadOnly;
        }

        if (value is LocalizationString)
        {
            return ReadOnly;
        }

        if (value is NodeRef)
        {
            return ReadOnly;
        }

        if (value is IRedInteger)
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
    public DataTemplate NodeRef { get; set; }
    public DataTemplate CEnum { get; set; }
    public DataTemplate CHandle { get; set; }
    public DataTemplate TweakDBID { get; set; }
    public DataTemplate RedInteger { get; set; }
    public DataTemplate RedInteger2 { get; set; }
    public DataTemplate LocalizationString { get; set; }
    public DataTemplate ResourcePath { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not PropertyViewModel node)
        {
            return ReadOnly;
        }

        var value = node.DataObject;
        if (value is CBool)
        {
            return CBool;
        }

        if (value is IRedRef)
        {
            return ResourcePath;
        }

        if (value is CName)
        {
            return CName;
        }

        if (value is CString)
        {
            return CString;
        }

        if (value is LocalizationString)
        {
            return LocalizationString;
        }

        if (value is NodeRef)
        {
            return NodeRef;
        }

        if (value is CInt64 or CUInt64 or CRUID)
        {
            return RedInteger2;
        }

        if (value is IRedInteger)
        {
            return RedInteger;
        }

        if (value is TweakDBID)
        {
            return TweakDBID;
        }

        if (value is IRedEnum)
        {
            return CEnum;
        }

        if (value is IRedHandle)
        {
            return CHandle;
        }

        if (value is LocalizationString)
        {
            return ReadOnly;
        }

        return ReadOnly;
    }
}