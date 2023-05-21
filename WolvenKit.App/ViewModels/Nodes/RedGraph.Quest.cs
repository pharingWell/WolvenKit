using System;
using System.Collections.Generic;
using System.Linq;
using WolvenKit.App.Factories;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes;

public partial class RedGraph
{
    private static List<Type>? s_questNodeTypes;

    private INodeWrapperFactory? _nodeWrapperFactory;

    
    public List<Type> GetQuestNodeTypes()
    {
        if (s_questNodeTypes == null)
        {
            s_questNodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(typeof(graphGraphNodeDefinition).IsAssignableFrom)
                .ToList();

            s_questNodeTypes.Remove(typeof(graphGraphNodeDefinition));
        }

        return s_questNodeTypes;
    }

    public void CreateQuestNode(Type type, System.Windows.Point point)
    {
        var instance = InternalCreateQuestNode(type);
        var wrappedInstance = WrapQuestNode(instance);
        wrappedInstance.Location = point;

        ((graphGraphDefinition)_data).Nodes.Add(new CHandle<graphGraphNodeDefinition>(instance));
        Nodes.Add(wrappedInstance);
    }

    private graphGraphNodeDefinition InternalCreateQuestNode(Type type)
    {
        var instance = System.Activator.CreateInstance(type);
        if (instance is not graphGraphNodeDefinition questNode)
        {
            throw new Exception();
        }

        return questNode;
    }

    private BaseQuestViewModel WrapQuestNode(graphGraphNodeDefinition node)
    {
        BaseQuestViewModel nodeWrapper;
        if (node is questInputNodeDefinition inputNode)
        {
            nodeWrapper = new questInputNodeDefinitionWrapper(inputNode);
        }
        else if (node is questOutputNodeDefinition outputNode)
        {
            nodeWrapper = new questOutputNodeDefinitionWrapper(outputNode);
        }
        else if (node is questPauseConditionNodeDefinition pauseCondition)
        {
            nodeWrapper = new questPauseConditionNodeDefinitionWrapper(pauseCondition);
        }
        else if (node is questFactsDBManagerNodeDefinition factsDbManager)
        {
            nodeWrapper = new questFactsDBManagerNodeDefinitionWrapper(factsDbManager);
        }
        else if (node is questRenderFxManagerNodeDefinition renderFxManager)
        {
            nodeWrapper = new questRenderFxManagerNodeDefinitionWrapper(renderFxManager);
        }
        else if (node is questSwitchNodeDefinition switchNode)
        {
            nodeWrapper = new questSwitchNodeDefinitionWrapper(switchNode);
        }
        else if (node is questPhaseNodeDefinition questPhase)
        {
            nodeWrapper = _nodeWrapperFactory!.QuestPhaseNodeDefinitionWrapper(questPhase);
        }
        else if (node is questSceneNodeDefinition scene)
        {
            nodeWrapper = _nodeWrapperFactory!.QuestSceneNodeDefinitionWrapper(scene);
        }
        else if (node is questRandomizerNodeDefinition randomizerNode)
        {
            nodeWrapper = new questRandomizerNodeDefinitionWrapper(randomizerNode);
        }
        else if (node is questPauseConditionNodeDefinition pauseConditionNode)
        {
            nodeWrapper = new questNodeDefinitionWrapper(pauseConditionNode);
        }
        else if (node is questNodeDefinition questNode)
        {
            nodeWrapper = new questNodeDefinitionWrapper(questNode);
        }
        else
        {
            nodeWrapper = new graphGraphNodeDefinitionWrapper(node);
        }

        nodeWrapper.GenerateSockets();
        
        return nodeWrapper;
    }

    public static RedGraph GenerateQuestGraph(string title, graphGraphDefinition questGraph, INodeWrapperFactory nodeWrapperFactory)
    {
        var graph = new RedGraph(title, questGraph);
        graph._nodeWrapperFactory = nodeWrapperFactory;

        var socketNodeLookup = new Dictionary<graphGraphSocketDefinition, QuestInputConnectorViewModel>();

        var nodeCache = new Dictionary<uint, BaseQuestViewModel>();
        foreach (var nodeHandle in questGraph.Nodes)
        {
            ArgumentNullException.ThrowIfNull(nodeHandle.Chunk);

            var node = nodeHandle.Chunk;

            var nvm = graph.WrapQuestNode(node);

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