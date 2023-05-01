using System.Collections.ObjectModel;
using System.Linq;
using WolvenKit.App.ViewModels.Nodes.Scene.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnChoiceNodeWrapper : BaseSceneViewModel<scnChoiceNode>
{
    private readonly scnSceneResource _sceneResource;

    public ObservableCollection<string> Options { get; set; } = new();

    public scnChoiceNodeWrapper(scnChoiceNode scnSceneGraphNode, scnSceneResource scnSceneResource) : base(scnSceneGraphNode)
    {
        _sceneResource = scnSceneResource;
    }

    internal override void GenerateSockets()
    {
        GetChoices();

        Input.Add(new SceneInputConnectorViewModel("In", "In", UniqueId, 0));

        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            var title = $"Out{i}";
            if (i < Options.Count)
            {
                title = Options[i];
            }

            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", title, UniqueId, _castedData.OutputSockets[i]));
        }
    }

    private void GetChoices()
    {
        foreach (var option in _castedData.Options)
        {
            var screenPlay = _sceneResource
                .ScreenplayStore
                .Options
                .First(x => x.ItemId.Id == option.ScreenplayOptionId.Id);

            var vdEntry = _sceneResource
                .LocStore
                .VdEntries
                .First(x => x.LocstringId.Ruid == screenPlay.LocstringId.Ruid);

            var vpEntry = _sceneResource
                .LocStore
                .VpEntries
                .First(x => x.VariantId.Ruid == vdEntry.VariantId.Ruid);

            Options.Add($"[{vdEntry.LocaleId.ToEnumString()}] {vpEntry.Content}");
        }
    }
}