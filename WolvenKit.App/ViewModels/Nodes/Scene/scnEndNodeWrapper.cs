using System.Collections.Generic;
using System;
using System.Linq;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnEndNodeWrapper : BaseSceneViewModel<scnEndNode>
{
    private readonly scnExitPoint _scnExitPoint;

    public string Name
    {
        get => _scnExitPoint.Name.GetResolvedText()!;
        set => _scnExitPoint.Name = value;
    }

    public Enums.scnEndNodeNsType Type
    {
        get => _castedData.Type; 
        set => _castedData.Type = value;
    }

    public IEnumerable<Enums.scnEndNodeNsType> Types => Enum.GetValues(typeof(Enums.scnEndNodeNsType)).Cast<Enums.scnEndNodeNsType>();

    public scnEndNodeWrapper(scnEndNode scnEndNode, scnExitPoint exitPoint) : base(scnEndNode) => _scnExitPoint = exitPoint;

    internal override void GenerateSockets() => Input.Add(new SceneInputConnectorViewModel("In", "In", NodeId, 0));
}