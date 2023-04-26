using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.Common;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes;

public class RedGraph
{
    public string Title { get; }
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

    public RedGraph(string title) => Title = title;

    public void ArrangeNodes(double xOffset = 0, double yOffset = 0)
    {
        var graph = new GeometryGraph();
        var msaglNodes = new Dictionary<uint, Node>();

        foreach (var node in Nodes)
        {
            var msaglNode = new Node(CurveFactory.CreateRectangle(node.Size.Width, node.Size.Height, new Microsoft.Msagl.Core.Geometry.Point()))
            {
                UserData = node
            };

            msaglNodes.Add(node.UniqueId, msaglNode);
            graph.Nodes.Add(msaglNode);
        }

        foreach (var connection in Connections)
        {
            graph.Edges.Add(new Edge(msaglNodes[connection.Source.OwnerId], msaglNodes[connection.Target.OwnerId]));
        }

        var settings = new SugiyamaLayoutSettings
        {
            Transformation = PlaneTransformation.Rotation(Math.PI / 2),
            EdgeRoutingSettings = { EdgeRoutingMode = EdgeRoutingMode.Spline }
        };

        var layout = new LayeredLayout(graph, settings);
        layout.Run();

        foreach (var node in graph.Nodes)
        {
            var nvm = (Nodes.NodeViewModel)node.UserData;
            nvm.Location = new System.Windows.Point(
                node.Center.X - graph.BoundingBox.Center.X - (nvm.Size.Width / 2) + xOffset,
                node.Center.Y - graph.BoundingBox.Center.Y - (nvm.Size.Height / 2) + yOffset);
        }
    }

    public static RedGraph GenerateQuestGraph(string title, graphGraphDefinition questGraph, INodeWrapperFactory nodeWrapperFactory)
    {
        var graph = new RedGraph(title);

        var socketNodeLookup = new Dictionary<int, int>();
        var connectionCache = new Dictionary<int, graphGraphConnectionDefinition>();

        var nodeCache = new Dictionary<uint, BaseQuestViewModel>();
        foreach (var nodeHandle in questGraph.Nodes)
        {
            ArgumentNullException.ThrowIfNull(nodeHandle.Chunk);

            var node = nodeHandle.Chunk;

            BaseQuestViewModel nvm;
            if (node is questPhaseNodeDefinition questPhase)
            {
                nvm = nodeWrapperFactory.QuestPhaseNodeDefinitionWrapper(questPhase);
            }
            else if (node is questSceneNodeDefinition scene)
            {
                nvm = nodeWrapperFactory.QuestSceneNodeDefinitionWrapper(scene);
            }
            else if (node is questRandomizerNodeDefinition randomizerNode)
            {
                nvm = new questRandomizerNodeDefinitionWrapper(randomizerNode);
            }
            else if (node is questInputNodeDefinition inputNode)
            {
                nvm = new questInputNodeDefinitionWrapper(inputNode);
            }
            else if (node is questNodeDefinition questNode)
            {
                nvm = new questNodeDefinitionWrapper(questNode);
            }
            else
            {
                nvm = new graphGraphNodeDefinitionWrapper(node);
            }

            nodeCache.Add(nvm.UniqueId, nvm);
            graph.Nodes.Add(nvm);

            foreach (var socketHandle in node.Sockets)
            {
                socketNodeLookup.Add(socketHandle.Chunk!.GetHashCode(), node.GetHashCode());
                foreach (var connection in socketHandle.Chunk!.Connections)
                {
                    connectionCache.TryAdd(connection.GetHashCode(), connection.Chunk!);
                }
            }
        }

        foreach (var (hash, connection) in connectionCache)
        {
            var source = (uint)socketNodeLookup[connection.Source.Chunk!.GetHashCode()];
            var srcNode = nodeCache[source];
            var srcConnector = srcNode.Output.First(x => x.Name == connection.Source.Chunk!.Name);

            var target = (uint)socketNodeLookup[connection.Destination.Chunk!.GetHashCode()];
            var targetNode = nodeCache[target];
            var targetConnector = targetNode.Input.First(x => x.Name == connection.Destination.Chunk!.Name);

            graph.Connections.Add(new Nodes.ConnectionViewModel(srcConnector, targetConnector));
        }

        return graph;
    }

    public static RedGraph GenerateSceneGraph(string title, scnSceneResource sceneResource)
    {
        var graph = new RedGraph(title);

        var nodeCache = new Dictionary<uint, BaseSceneViewModel>();
        foreach (var nodeHandle in sceneResource.SceneGraph.Chunk!.Graph)
        {
            ArgumentNullException.ThrowIfNull(nodeHandle.Chunk);

            var node = nodeHandle.Chunk;

            BaseSceneViewModel nvm;
            if (node is scnRandomizerNode randomizerNode)
            {
                nvm = new scnRandomizerNodeWrapper(randomizerNode);
            }
            else if (node is scnStartNode startNode)
            {
                var startName = sceneResource
                    .EntryPoints
                    .FirstOrDefault(x => x.NodeId.Id == startNode.NodeId.Id)!
                    .Name.GetResolvedText()!;

                nvm = new scnStartNodeWrapper(startNode, startName);
            }
            else if (node is scnEndNode endNode)
            {
                var endName = sceneResource
                    .ExitPoints
                    .FirstOrDefault(x => x.NodeId.Id == endNode.NodeId.Id)!
                    .Name.GetResolvedText()!;

                nvm = new scnEndNodeWrapper(endNode, endName);
            }
            else if (node is scnXorNode xorNode)
            {
                nvm = new scnXorNodeWrapper(xorNode);
            }
            else if (node is scnQuestNode questNode)
            {
                nvm = new scnQuestNodeWrapper(questNode);
            }
            else if (node is scnSectionNode sectionNode)
            {
                nvm = new scnSectionNodeWrapper(sectionNode);
            }
            else
            {
                nvm = new scnSceneGraphNodeWrapper(node);
            }

            nodeCache.Add(nvm.UniqueId, nvm);
            graph.Nodes.Add(nvm);
        }

        foreach (var node in graph.Nodes)
        {
            foreach (var outputConnector in node.Output)
            {
                foreach (var targetId in outputConnector.TargetIds)
                {
                    graph.Connections.Add(new Nodes.ConnectionViewModel(outputConnector, nodeCache[targetId.Item1].Input[targetId.Item2]));
                }
            }
        }

        return graph;
    }
}