using System.Collections.Generic;
using System;
using System.Linq;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.RED4.Types;
using SharpDX.DXGI;

namespace WolvenKit.App.ViewModels.Nodes;

public partial class RedGraph
{
    public void CreateSceneNode<T>() where T : scnSceneGraphNode
    {
        var instance = System.Activator.CreateInstance<T>();

        instance.NodeId.Id = ++_currentSceneNodeId;
        instance.OutputSockets.Add(new scnOutputSocket { Stamp = new scnOutputSocketStamp { Name = 0, Ordinal = 0 } });

        ((scnSceneResource)_data).SceneGraph.Chunk!.Graph.Add(new CHandle<scnSceneGraphNode>(instance));
        Nodes.Add(WrapSceneNode(instance));
    }

    private void RemoveSceneNode(BaseSceneViewModel node)
    {
        for (var i = Connections.Count - 1; i >= 0; i--)
        {
            if (node.Output.Count == 0)
            {
                break;
            }

            for (var j = node.Output.Count - 1; j >= 0; j--)
            {
                if (Connections[i].Source == node.Output[j])
                {
                    node.Output.RemoveAt(j);
                    Connections.RemoveAt(i);
                    break;
                }
            }
        }

        for (var i = Connections.Count - 1; i >= 0; i--)
        {
            if (node.Input.Count == 0)
            {
                break;
            }

            for (var j = node.Input.Count - 1; j >= 0; j--)
            {
                if (Connections[i].Target == node.Input[j])
                {
                    var connectionSource = (SceneOutputConnectorViewModel)Connections[i].Source;
                    for (var k = connectionSource.Data.Destinations.Count - 1; k >= 0; k--)
                    {
                        if (connectionSource.Data.Destinations[k].NodeId.Id == node.NodeId)
                        {
                            connectionSource.Data.Destinations.RemoveAt(k);
                        }
                    }

                    node.Input.RemoveAt(j);
                    Connections.RemoveAt(i);
                    break;
                }
            }
        }

        var graph = ((scnSceneResource)_data).SceneGraph.Chunk!.Graph!;
        for (var i = graph.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(graph[i].Chunk, node.Data))
            {
                graph.RemoveAt(i);
            }
        }

        Nodes.Remove(node);
    }

    private BaseSceneViewModel WrapSceneNode(scnSceneGraphNode node)
    {
        var sceneResource = (scnSceneResource)_data;

        if (node is scnAndNode andNode)
        {
            return new scnAndNodeWrapper(andNode);
        }

        if (node is scnChoiceNode choiceNode)
        {
            return new scnChoiceNodeWrapper(choiceNode);
        }

        if (node is scnCutControlNode cutControlNode)
        {
            return new scnCutControlNodeWrapper(cutControlNode);
        }

        if (node is scnDeletionMarkerNode deletionMarkerNode)
        {
            return new scnDeletionMarkerNodeWrapper(deletionMarkerNode);
        }

        if (node is scnEndNode endNode)
        {
            var endName = sceneResource
                .ExitPoints
                .FirstOrDefault(x => x.NodeId.Id == endNode.NodeId.Id)!
                .Name.GetResolvedText()!;

            return new scnEndNodeWrapper(endNode, endName);
        }

        if (node is scnHubNode hubNode)
        {
            return new scnHubNodeWrapper(hubNode);
        }

        if (node is scnInterruptManagerNode interruptManagerNode)
        {
            return new scnInterruptManagerNodeWrapper(interruptManagerNode);
        }

        if (node is scnQuestNode questNode)
        {
            return new scnQuestNodeWrapper(questNode);
        }

        if (node is scnRandomizerNode randomizerNode)
        {
            return new scnRandomizerNodeWrapper(randomizerNode);
        }

        if (node is scnRewindableSectionNode rewindableSectionNode)
        {
            return new scnRewindableSectionNodeWrapper(rewindableSectionNode);
        }

        if (node is scnSectionNode sectionNode)
        {
            return new scnSectionNodeWrapper(sectionNode);
        }

        if (node is scnStartNode startNode)
        {
            var startName = sceneResource
                .EntryPoints
                .FirstOrDefault(x => x.NodeId.Id == startNode.NodeId.Id)!
                .Name.GetResolvedText()!;

            return new scnStartNodeWrapper(startNode, startName);
        }

        if (node is scnXorNode xorNode)
        {
            return new scnXorNodeWrapper(xorNode);
        }

        // shouldn't happen, just for failsafe
        return new scnSceneGraphNodeWrapper(node);
    }

    public static RedGraph GenerateSceneGraph(string title, scnSceneResource sceneResource)
    {
        var graph = new RedGraph(title, sceneResource);

        var nodeCache = new Dictionary<uint, BaseSceneViewModel>();
        foreach (var nodeHandle in sceneResource.SceneGraph.Chunk!.Graph)
        {
            ArgumentNullException.ThrowIfNull(nodeHandle.Chunk);

            var node = nodeHandle.Chunk;

            var nvm = graph.WrapSceneNode(node);

            nodeCache.Add(nvm.UniqueId, nvm);
            graph.Nodes.Add(nvm);

            graph._currentSceneNodeId = Math.Max(graph._currentSceneNodeId, nvm.NodeId);
        }

        foreach (var node in graph.Nodes)
        {
            var sceneNode = (BaseSceneViewModel)node;

            foreach (var outputConnector in sceneNode.Output)
            {
                var sceneOutputConnector = (SceneOutputConnectorViewModel)outputConnector;

                foreach (var destination in sceneOutputConnector.Data.Destinations)
                {
                    var targetNode = nodeCache[destination.NodeId.Id];
                    if (targetNode is IDynamicInputNode dynamicInputNode)
                    {
                        while (dynamicInputNode.Input.Count <= destination.IsockStamp.Ordinal)
                        {
                            dynamicInputNode.AddInput();
                        }
                    }

                    graph.Connections.Add(new SceneConnectionViewModel(outputConnector, targetNode.Input[destination.IsockStamp.Ordinal]));
                }
            }
        }

        return graph;
    }
}