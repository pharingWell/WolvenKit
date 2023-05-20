using System;
using Syncfusion.Data;
using System.Collections.Generic;
using System.ComponentModel;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.App.ViewModels.Shell.RedTypes;

namespace WolvenKit.Helpers;

public class PropertyViewComparer : IComparer<object>, ISortDirection
{
    public int Compare(object x, object y)
    {
        if (x is not PropertyViewModel p1 || y is not PropertyViewModel p2)
        {
            throw new Exception();
        }

        var value1 = p1.DisplayName;
        var value2 = p2.DisplayName;

        int c;

        if (p1.RedPropertyInfo.Index != -1)
        {
            c = p1.RedPropertyInfo.Index.CompareTo(p2.RedPropertyInfo.Index);
        }
        else
        {
            c = string.Compare(value1, value2, StringComparison.InvariantCultureIgnoreCase);
        }

        if (SortDirection == ListSortDirection.Descending)
        {
            c = -c;
        }

        return c;
    }

    public ListSortDirection SortDirection { get; set; }
}