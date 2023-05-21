﻿using System;
using System.ComponentModel;
using System.IO;
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.Common;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questPhaseNodeDefinitionWrapper : BaseQuestViewModel<questPhaseNodeDefinition>, IGraphProvider
{
    private readonly INodeWrapperFactory _nodeWrapperFactory;
    private readonly IArchiveManager _archiveManager;

    private RedGraph? _graph = null;

    public RedGraph Graph
    {
        get
        {
            if (_graph == null)
            {
                GenerateSubGraph();
            }
            return _graph!;
        }
    }


    public questPhaseNodeDefinitionWrapper(questPhaseNodeDefinition questPhaseNodeDefinition, INodeWrapperFactory nodeWrapperFactory, IArchiveManager archiveManager) : base(questPhaseNodeDefinition)
    {
        _nodeWrapperFactory = nodeWrapperFactory;
        _archiveManager = archiveManager;

        Title = $"{Title} [{questPhaseNodeDefinition.Id}]";
        if (_castedData.PhaseResource.DepotPath != ResourcePath.Empty && _castedData.PhaseResource.DepotPath.IsResolvable)
        {
            Details.Add("Filename", Path.GetFileName(_castedData.PhaseResource.DepotPath.GetResolvedText())!);
        }
    }

    private void GenerateSubGraph()
    {
        if (_castedData.PhaseGraph != null && _castedData.PhaseGraph.Chunk != null)
        {
            _graph = RedGraph.GenerateQuestGraph(Title, _castedData.PhaseGraph.Chunk, _nodeWrapperFactory);
        }
        else if (_castedData.PhaseResource.DepotPath != ResourcePath.Empty)
        {
            var cr2w = _archiveManager.GetGameFile(_castedData.PhaseResource.DepotPath);

            if (cr2w is not { RootChunk: questQuestPhaseResource res } || res.Graph?.Chunk == null)
            {
                throw new Exception();
            }

            var fileName = ((ulong)_castedData.PhaseResource.DepotPath).ToString();
            if (_castedData.PhaseResource.DepotPath.IsResolvable)
            {
                fileName = _castedData.PhaseResource.DepotPath.GetResolvedText();
            }

            _graph = RedGraph.GenerateQuestGraph(fileName!, res.Graph.Chunk, _nodeWrapperFactory);
        }
        else
        {
            throw new Exception();
        }
    }

    public void RecalculateSockets()
    {
        _castedData.Sockets.Clear();
        _castedData.Sockets.Add(new CHandle<graphGraphSocketDefinition>(new questSocketDefinition
        {
            Name = "CutDestination",
            Type = Enums.questSocketType.CutDestination
        }));

        if (_castedData.PhaseResource.DepotPath != ResourcePath.Empty)
        {
            var cr2w = _archiveManager.GetGameFile(_castedData.PhaseResource.DepotPath);

            if (cr2w is not { RootChunk: questQuestPhaseResource res } || res.Graph?.Chunk == null)
            {
                throw new Exception();
            }

            foreach (var nodeHandle in res.Graph.Chunk.Nodes)
            {
                if (nodeHandle.Chunk is questInputNodeDefinition inputNode)
                {
                    _castedData.Sockets.Add(new CHandle<graphGraphSocketDefinition>(new questSocketDefinition
                    {
                        Name = inputNode.SocketName,
                        Type = Enums.questSocketType.Input
                    }));
                }

                if (nodeHandle.Chunk is questOutputNodeDefinition outputNode)
                {
                    _castedData.Sockets.Add(new CHandle<graphGraphSocketDefinition>(new questSocketDefinition
                    {
                        Name = outputNode.SocketName,
                        Type = Enums.questSocketType.Output
                    }));
                }
            }
        }

        GenerateSockets();
    }
}