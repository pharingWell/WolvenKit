using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questFactsDBManagerNodeDefinitionWrapper : BaseQuestDisableableNodeDefinitionWrapper<questFactsDBManagerNodeDefinition>
{
    private questSetVar_NodeType _helper => (questSetVar_NodeType)_castedData.Type.Chunk!;

    public string FactName
    {
        get => _helper.FactName;
        set => _helper.FactName = value;
    }

    public int Value
    {
        get => _helper.Value;
        set => _helper.Value = value;
    }

    public bool SetExactValue
    {
        get => _helper.SetExactValue;
        set => _helper.SetExactValue = value;
    }

    public questFactsDBManagerNodeDefinitionWrapper(questFactsDBManagerNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }
}