using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.LargeGraphLayout;
using Microsoft.Msagl.Layout.Layered;
using WolvenKit.App.ViewModels.Nodes;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.RED4.Types;


namespace WolvenKit.App.ViewModels.Documents;

public partial class RDTGraphViewModel2 : RedDocumentTabViewModel
{
    protected readonly IRedType _data;

    [ObservableProperty]
    private GraphViewModel _mainGraph;

    public RDTGraphViewModel2(IRedType data, RedDocumentViewModel file) : base(file, "Graph View")
    {
        _data = data;
        _mainGraph = new GraphViewModel();
    }

    public override ERedDocumentItemType DocumentItemType => ERedDocumentItemType.MainFile;

    public void Load()
    {
        GraphViewModel? mainGraph = null;

        if (_data is graphGraphResource questResource)
        {
            if (questResource.Graph.Chunk is { } questGraph)
            {
                mainGraph = GraphViewModel.GenerateQuestGraph(questGraph);
            }
        }

        if (_data is scnSceneResource sceneResource)
        {
            mainGraph = GraphViewModel.GenerateSceneGraph(sceneResource);
        }

        if (mainGraph == null)
        {
            mainGraph = new GraphViewModel();
        }

        MainGraph = mainGraph;
    }
}