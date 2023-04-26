﻿using System;
using System.IO;
using WolvenKit.App.Factories;
using WolvenKit.Common;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Types;
using EFileReadErrorCodes = WolvenKit.RED4.Archive.IO.EFileReadErrorCodes;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questSceneNodeDefinitionWrapper : BaseQuestViewModel<questSceneNodeDefinition>, IGraphProvider
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

    public questSceneNodeDefinitionWrapper(questSceneNodeDefinition nodeDefinition, INodeWrapperFactory nodeWrapperFactory, IArchiveManager archiveManager) : base(nodeDefinition)
    {
        _nodeWrapperFactory = nodeWrapperFactory;
        _archiveManager = archiveManager;

        Title = $"{Title} [{nodeDefinition.Id}]";
        if (_castedData.SceneFile.DepotPath != ResourcePath.Empty && _castedData.SceneFile.DepotPath.IsResolvable)
        {
            Details.Add("Filename", Path.GetFileName(_castedData.SceneFile.DepotPath.GetResolvedText())!);
        }
    }

    private void GenerateSubGraph()
    {
        if (_castedData.SceneFile.DepotPath != ResourcePath.Empty && _castedData.SceneFile.DepotPath.IsResolvable)
        {
            var file = _archiveManager.Lookup(_castedData.SceneFile.DepotPath);
            if (!file.HasValue)
            {
                throw new Exception();
            }

            using var ms = new MemoryStream();
            file.Value.Extract(ms);
            ms.Position = 0;

            using var reader = new CR2WReader(ms);
            if (reader.ReadFile(out var cr2w) != EFileReadErrorCodes.NoError)
            {
                throw new Exception();
            }

            if (cr2w!.RootChunk is not scnSceneResource res)
            {
                throw new Exception();
            }

            _graph = RedGraph.GenerateSceneGraph(Path.GetFileName(file.Value.FileName), res);
        }
        else
        {
            throw new Exception();
        }
    }
}