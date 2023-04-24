using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WolvenKit.App.ViewModels.Nodes;

public class OutputConnectorViewModel : BaseConnectorViewModel
{
    public ObservableCollection<(uint, ushort)> TargetIds { get; }

    public OutputConnectorViewModel(string title, uint ownerId, IList<(uint, ushort)> targetIds) : base(title, ownerId)
    {
        TargetIds = new ObservableCollection<(uint, ushort)>(targetIds);
    }
}