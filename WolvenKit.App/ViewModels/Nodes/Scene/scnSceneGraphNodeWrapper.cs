﻿using System.Collections.Generic;
using WolvenKit.App.ViewModels.Nodes.Scene.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnSceneGraphNodeWrapper : BaseSceneViewModel<scnSceneGraphNode>
{
    public scnSceneGraphNodeWrapper(scnSceneGraphNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }
}