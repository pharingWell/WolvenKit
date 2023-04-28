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
using WolvenKit.RED4.Types;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace WolvenKit.App.ViewModels.Nodes;

public class RedGraph
{
    private readonly IRedType _data;

    public string Title { get; }
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
    public PendingConnectionViewModel PendingConnection { get; }

    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }

    public RedGraph(string title, IRedType data)
    {
        _data = data;

        Title = title;
        PendingConnection = new PendingConnectionViewModel();

        ConnectCommand = new RelayCommand(Connect);
        DisconnectCommand = new RelayCommand<BaseConnectorViewModel>(Disconnect);
    }

    public void Connect()
    {
        if (PendingConnection.Target == null)
        {
            return;
        }

        if (_data is graphGraphDefinition questDefinition)
        {
            var sceneSource = (QuestOutputConnectorViewModel)PendingConnection.Source;
            var sceneTarget = (QuestInputConnectorViewModel)PendingConnection.Target;

            var graphGraphConnectionDefinition = new graphGraphConnectionDefinition
            {
                Source = new CWeakHandle<graphGraphSocketDefinition>(sceneSource.Data),
                Destination = new CWeakHandle<graphGraphSocketDefinition>(sceneTarget.Data)
            };
            var handle = new CHandle<graphGraphConnectionDefinition>(graphGraphConnectionDefinition);

            sceneSource.Data.Connections.Add(handle);
            sceneTarget.Data.Connections.Add(handle);

            Connections.Add(new ConnectionViewModel(sceneSource, sceneTarget));
        }

        if (_data is scnSceneResource sceneResource)
        {
            var sceneSource = (SceneOutputConnectorViewModel)PendingConnection.Source;
            var sceneTarget = (SceneInputConnectorViewModel)PendingConnection.Target;

            var input = new scnInputSocketId
            {
                NodeId = new scnNodeId
                {
                    Id = sceneTarget.OwnerId
                },
                IsockStamp = new scnInputSocketStamp
                {
                    Name = 0,
                    Ordinal = sceneTarget.Ordinal
                }
            };

            sceneSource.Data.Destinations.Add(input);
            Connections.Add(new ConnectionViewModel(sceneSource, sceneTarget));
        }

        //Connections.Add(new ConnectionViewModel(source, target));
    }

    private void Disconnect(BaseConnectorViewModel? baseConnectorViewModel)
    {
        if (baseConnectorViewModel is OutputConnectorViewModel outputConnector)
        {
            for (var i = Connections.Count - 1; i >= 0; i--)
            {
                if (Connections[i].Source == outputConnector)
                {
                    RemoveConnection(Connections[i]);
                }
            }
        }

        if (baseConnectorViewModel is InputConnectorViewModel inputConnector)
        {
            for (var i = Connections.Count - 1; i >= 0; i--)
            {
                if (Connections[i].Target == inputConnector)
                {
                    RemoveConnection(Connections[i]);
                }
            }
        }
    }

    private void RemoveConnection(ConnectionViewModel connection)
    {
        if (connection is QuestConnectionViewModel questConnection)
        {
            var questSource = (QuestOutputConnectorViewModel)connection.Source;
            var questDestination = (QuestInputConnectorViewModel)connection.Target;

            for (var i = questSource.Data.Connections.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(questSource.Data.Connections[i].Chunk, questConnection.ConnectionDefinition))
                {
                    questSource.Data.Connections.RemoveAt(i);
                }
            }

            for (var i = questDestination.Data.Connections.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(questDestination.Data.Connections[i].Chunk, questConnection.ConnectionDefinition))
                {
                    questDestination.Data.Connections.RemoveAt(i);
                }
            }
            Connections.Remove(connection);
        }

        if (_data is scnSceneResource sceneResource)
        {
            var sceneSource = (SceneOutputConnectorViewModel)connection.Source;
            var sceneDestination = sceneSource.Data.Destinations.FirstOrDefault(x => x.NodeId.Id == connection.Target.OwnerId);

            sceneSource.Data.Destinations.Remove(sceneDestination);
            Connections.Remove(connection);
        }
    }

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
            var nvm = (NodeViewModel)node.UserData;
            nvm.Location = new System.Windows.Point(
                node.Center.X - graph.BoundingBox.Center.X - (nvm.Size.Width / 2) + xOffset,
                node.Center.Y - graph.BoundingBox.Center.Y - (nvm.Size.Height / 2) + yOffset);
        }
    }

    public static RedGraph GenerateQuestGraph(string title, graphGraphDefinition questGraph, INodeWrapperFactory nodeWrapperFactory)
    {
        var graph = new RedGraph(title, questGraph);

        var socketNodeLookup = new Dictionary<graphGraphSocketDefinition, QuestInputConnectorViewModel>();
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
            else if (node is questPauseConditionNodeDefinition pauseConditionNode)
            {
                nvm = new questNodeDefinitionWrapper(pauseConditionNode);
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

            foreach (var inputConnector in nvm.Input)
            {
                var questInputConnector = (QuestInputConnectorViewModel)inputConnector;
                socketNodeLookup.Add(questInputConnector.Data, questInputConnector);
            }
        }

        foreach (var node in graph.Nodes)
        {
            var questNode = (BaseQuestViewModel)node;

            foreach (var outputConnector in questNode.Output)
            {
                var questOutputConnector = (QuestOutputConnectorViewModel)outputConnector;

                foreach (var connectionHandle in questOutputConnector.Data.Connections)
                {
                    var connection = connectionHandle.Chunk!;

                    graph.Connections.Add(new QuestConnectionViewModel(questOutputConnector, socketNodeLookup[connection.Destination.Chunk!], connection));
                }
            }
        }

        return graph;
    }

    public static RedGraph GenerateSceneGraph(string title, scnSceneResource sceneResource)
    {
        var graph = new RedGraph(title, sceneResource);

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
            var sceneNode = (BaseSceneViewModel)node;

            foreach (var outputConnector in sceneNode.Output)
            {
                var sceneOutputConnector = (SceneOutputConnectorViewModel)outputConnector;

                foreach (var destination in sceneOutputConnector.Data.Destinations)
                {
                    graph.Connections.Add(new ConnectionViewModel(outputConnector, nodeCache[destination.NodeId.Id].Input[destination.IsockStamp.Ordinal]));
                }
            }
        }

        return graph;
    }
}