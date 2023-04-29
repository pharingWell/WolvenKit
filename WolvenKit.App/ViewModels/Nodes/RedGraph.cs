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

public enum RedGraphType
{
    Invalid,
    Quest,
    Scene
}

public partial class RedGraph
{
    private IRedType _data;

    private uint _currentSceneNodeId;

    public RedGraphType GraphType { get; } = RedGraphType.Invalid;

    public string Title { get; }
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
    public PendingConnectionViewModel PendingConnection { get; }

    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }

    public RedGraph(string title, IRedType data)
    {
        _data = data;
        if (_data is graphGraphDefinition)
        {
            GraphType = RedGraphType.Quest;
        }
        else if (_data is scnSceneResource)
        {
            GraphType = RedGraphType.Scene;
        }

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

        if (GraphType == RedGraphType.Quest)
        {
            QuestInputConnectorViewModel questTarget;
            if (PendingConnection.Target is QuestInputConnectorViewModel questInput)
            {
                questTarget = questInput;
            } 
            else if (PendingConnection.Target is IDynamicInputNode dynamicInput)
            {
                questTarget = (QuestInputConnectorViewModel)dynamicInput.AddInput();
            }
            else
            {
                return;
            }

            var questSource = (QuestOutputConnectorViewModel)PendingConnection.Source;

            for (var i = Connections.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(Connections[i].Source, questSource) ||
                    !ReferenceEquals(Connections[i].Target, questTarget))
                {
                    continue;
                }

                for (var j = questSource.Data.Connections.Count - 1; j >= 0; j--)
                {
                    if (ReferenceEquals(questSource.Data.Connections[i].Chunk!.Destination.Chunk, questTarget.Data))
                    {
                        questSource.Data.Connections.RemoveAt(j);
                    }
                }

                for (var j = questTarget.Data.Connections.Count - 1; j >= 0; j--)
                {
                    if (ReferenceEquals(questTarget.Data.Connections[i].Chunk!.Source.Chunk, questSource.Data))
                    {
                        questTarget.Data.Connections.RemoveAt(j);
                    }
                }

                Connections.RemoveAt(i);

                return;
            }

            var graphGraphConnectionDefinition = new graphGraphConnectionDefinition
            {
                Source = new CWeakHandle<graphGraphSocketDefinition>(questSource.Data),
                Destination = new CWeakHandle<graphGraphSocketDefinition>(questTarget.Data)
            };
            var handle = new CHandle<graphGraphConnectionDefinition>(graphGraphConnectionDefinition);

            questSource.Data.Connections.Add(handle);
            questTarget.Data.Connections.Add(handle);

            Connections.Add(new QuestConnectionViewModel(questSource, questTarget, graphGraphConnectionDefinition));
        }

        if (GraphType == RedGraphType.Scene)
        {
            SceneInputConnectorViewModel sceneTarget;
            if (PendingConnection.Target is SceneInputConnectorViewModel questInput)
            {
                sceneTarget = questInput;
            }
            else if (PendingConnection.Target is IDynamicInputNode dynamicInput)
            {
                sceneTarget = (SceneInputConnectorViewModel)dynamicInput.AddInput();
            }
            else
            {
                return;
            }

            var sceneSource = (SceneOutputConnectorViewModel)PendingConnection.Source;

            for (var i = Connections.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(Connections[i].Source, sceneSource) ||
                    !ReferenceEquals(Connections[i].Target, sceneTarget))
                {
                    continue;
                }

                for (var j = sceneSource.Data.Destinations.Count - 1; j >= 0; j--)
                {
                    if (sceneSource.Data.Destinations[j].NodeId.Id == sceneTarget.OwnerId &&
                        sceneSource.Data.Destinations[j].IsockStamp.Ordinal == sceneTarget.Ordinal)
                    {
                        sceneSource.Data.Destinations.RemoveAt(j);
                    }
                }

                Connections.RemoveAt(i);

                return;
            }

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
            Connections.Add(new SceneConnectionViewModel(sceneSource, sceneTarget));
        }
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
        if (GraphType == RedGraphType.Quest)
        {
            var questConnection = (QuestConnectionViewModel)connection;

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
            Connections.Remove(questConnection);
        }

        if (GraphType == RedGraphType.Scene)
        {
            var sceneConnection = (SceneConnectionViewModel)connection;

            var sceneSource = (SceneOutputConnectorViewModel)sceneConnection.Source;
            var sceneDestination = sceneSource.Data.Destinations.FirstOrDefault(x => x.NodeId.Id == sceneConnection.Target.OwnerId);

            sceneSource.Data.Destinations.Remove(sceneDestination);
            Connections.Remove(sceneConnection);
        }
    }

    public void RemoveNode(NodeViewModel node)
    {
        if (!Nodes.Contains(node))
        {
            return;
        }

        if (GraphType == RedGraphType.Scene && node is BaseSceneViewModel sceneNode)
        {
            RemoveSceneNode(sceneNode);
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
}