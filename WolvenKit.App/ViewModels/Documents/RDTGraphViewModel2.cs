using System;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes;
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
        _mainGraph = new RedGraph("ERROR", new RedDummy());
    }

    public override ERedDocumentItemType DocumentItemType => ERedDocumentItemType.MainFile;

    public void Load()
    {
        MainGraph.Dispose();

        RedGraph? mainGraph = null;

        try
        {
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
        }
        catch (Exception)
        {
            throw;
        }

        if (mainGraph == null)
        {
            mainGraph = new RedGraph("ERROR", new RedDummy());
        }

        MainGraph = mainGraph;
    }
}