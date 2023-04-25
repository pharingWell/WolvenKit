using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WolvenKit.App.ViewModels.Nodes;

public class OutputConnectorViewModel : BaseConnectorViewModel
{
    public ObservableCollection<(uint, ushort)> TargetIds { get; }

    public OutputConnectorViewModel(string title, uint ownerId, IList<(uint, ushort)> targetIds) : this(title, title, ownerId, targetIds)
    {
    }

    public OutputConnectorViewModel(string name, string title, uint ownerId, IList<(uint, ushort)> targetIds) : base(name, title, ownerId)
    {
        TargetIds = new ObservableCollection<(uint, ushort)>(targetIds);
    }
}