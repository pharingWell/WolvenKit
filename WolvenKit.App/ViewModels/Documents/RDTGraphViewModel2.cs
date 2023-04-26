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
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.Common;
using WolvenKit.RED4.Types;


namespace WolvenKit.App.ViewModels.Documents;

public partial class RDTGraphViewModel2 : RedDocumentTabViewModel
{
    private readonly INodeWrapperFactory _nodeWrapperFactory;

    protected readonly IRedType _data;

    [ObservableProperty]
    private RedGraph _mainGraph;

    public RDTGraphViewModel2(IRedType data, RedDocumentViewModel file, INodeWrapperFactory nodeWrapperFactory) : base(file, "Graph View")
    {
        _nodeWrapperFactory = nodeWrapperFactory;

        _data = data;
        _mainGraph = new RedGraph("ERROR");
    }

    public override ERedDocumentItemType DocumentItemType => ERedDocumentItemType.MainFile;

    public void Load()
    {
        RedGraph? mainGraph = null;

        if (_data is graphGraphResource questResource)
        {
            if (questResource.Graph.Chunk is { } questGraph)
            {
                mainGraph = RedGraph.GenerateQuestGraph(Parent.Header, questGraph, _nodeWrapperFactory);
            }
        }

        if (_data is scnSceneResource sceneResource)
        {
            mainGraph = RedGraph.GenerateSceneGraph(Parent.Header, sceneResource);
        }

        if (mainGraph == null)
        {
            mainGraph = new RedGraph("ERROR");
        }

        MainGraph = mainGraph;
    }
}