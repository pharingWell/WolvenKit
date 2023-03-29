using System;
using System.Collections.Specialized;
using System.ComponentModel;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class CMeshViewModel : ClassPropertyViewModel<CMesh>
{
    public CMeshViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, CMesh? data) : base(parent, redPropertyInfo, data)
    {
    }
}