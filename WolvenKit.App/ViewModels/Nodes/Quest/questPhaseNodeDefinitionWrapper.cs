using System;
using System.IO;
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.Common;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Types;
using EFileReadErrorCodes = WolvenKit.RED4.Archive.IO.EFileReadErrorCodes;

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
            var file = _archiveManager.Lookup(_castedData.PhaseResource.DepotPath);
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

            if (cr2w!.RootChunk is not questQuestPhaseResource res || res.Graph == null || res.Graph.Chunk == null)
            {
                throw new Exception();
            }

            _graph = RedGraph.GenerateQuestGraph(Path.GetFileName(file.Value.FileName), res.Graph.Chunk, _nodeWrapperFactory);
        }
        else
        {
            throw new Exception();
        }
    }
}