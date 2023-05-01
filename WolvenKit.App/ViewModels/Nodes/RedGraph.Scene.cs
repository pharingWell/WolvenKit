using System.Collections.Generic;
using System;
using System.Linq;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.RED4.Types;
using SharpDX.DXGI;
using WolvenKit.App.ViewModels.Nodes.Scene.Internal;

namespace WolvenKit.App.ViewModels.Nodes;

public partial class RedGraph
{
    public void CreateSceneNode<T>() where T : scnSceneGraphNode => CreateSceneNode<T>(new System.Windows.Point());

    public void CreateSceneNode<T>(System.Windows.Point point) where T : scnSceneGraphNode
    {
        var instance = System.Activator.CreateInstance<T>();

        instance.NodeId.Id = ++_currentSceneNodeId;
        instance.OutputSockets.Add(new scnOutputSocket { Stamp = new scnOutputSocketStamp { Name = 0, Ordinal = 0 } });

        var wrappedInstance = WrapSceneNode(instance);
        wrappedInstance.Location = point;

        ((scnSceneResource)_data).SceneGraph.Chunk!.Graph.Add(new CHandle<scnSceneGraphNode>(instance));
        Nodes.Add(wrappedInstance);
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
                        if (connectionSource.Data.Destinations[k].NodeId.Id == node.UniqueId)
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

        BaseSceneViewModel nodeWrapper;
        if (node is scnAndNode andNode)
        {
            nodeWrapper = new scnAndNodeWrapper(andNode);
        } 
        else if (node is scnChoiceNode choiceNode)
        {
            nodeWrapper = new scnChoiceNodeWrapper(choiceNode, sceneResource);
        } 
        else if (node is scnCutControlNode cutControlNode)
        {
            nodeWrapper = new scnCutControlNodeWrapper(cutControlNode);
        } 
        else if (node is scnDeletionMarkerNode deletionMarkerNode)
        {
            nodeWrapper = new scnDeletionMarkerNodeWrapper(deletionMarkerNode);
        }
        else if (node is scnEndNode endNode)
        {
            var endPoint = sceneResource
                .ExitPoints
                .FirstOrDefault(x => x.NodeId.Id == endNode.NodeId.Id);

            if (endPoint == null)
            {
                endPoint = new scnExitPoint
                {
                    NodeId = new scnNodeId
                    {
                        Id = endNode.NodeId.Id
                    }
                };
                sceneResource.ExitPoints.Add(endPoint);
            }

            nodeWrapper = new scnEndNodeWrapper(endNode, endPoint);
        }
        else if (node is scnHubNode hubNode)
        {
            nodeWrapper = new scnHubNodeWrapper(hubNode);
        }
        else if (node is scnInterruptManagerNode interruptManagerNode)
        {
            nodeWrapper = new scnInterruptManagerNodeWrapper(interruptManagerNode);
        }
        else if (node is scnQuestNode questNode)
        {
            nodeWrapper = new scnQuestNodeWrapper(questNode);
        }
        else if (node is scnRandomizerNode randomizerNode)
        {
            nodeWrapper = new scnRandomizerNodeWrapper(randomizerNode);
        }
        else if (node is scnRewindableSectionNode rewindableSectionNode)
        {
            nodeWrapper = new scnRewindableSectionNodeWrapper(rewindableSectionNode);
        }
        else if (node is scnSectionNode sectionNode)
        {
            nodeWrapper = new scnSectionNodeWrapper(sectionNode);
        }
        else if (node is scnStartNode startNode)
        {
            var entryPoint = sceneResource
                .EntryPoints
                .FirstOrDefault(x => x.NodeId.Id == startNode.NodeId.Id);

            if (entryPoint == null)
            {
                entryPoint = new scnEntryPoint { 
                    NodeId = new scnNodeId
                    {
                        Id = startNode.NodeId.Id
                    }
                };
                sceneResource.EntryPoints.Add(entryPoint);
            }

            nodeWrapper = new scnStartNodeWrapper(startNode, entryPoint);
        }
        else if (node is scnXorNode xorNode)
        {
            nodeWrapper = new scnXorNodeWrapper(xorNode);
        }
        else
        {
            // shouldn't happen, just for failsafe
            nodeWrapper = new scnSceneGraphNodeWrapper(node);
        }

        nodeWrapper.GenerateSockets();

        return nodeWrapper;
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

            graph._currentSceneNodeId = Math.Max(graph._currentSceneNodeId, nvm.UniqueId);
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