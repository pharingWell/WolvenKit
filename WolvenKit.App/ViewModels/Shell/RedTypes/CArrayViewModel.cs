using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WolvenKit.App.Models.Nodify;
using WolvenKit.App.ViewModels.Documents;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public partial class CArrayViewModel : ChunkViewModel
{
    public CArrayViewModel(IRedType data, string name, ChunkViewModel? parent = null, bool isReadOnly = false) : base(data, name, parent, isReadOnly)
    {
        Init();
    }

    public CArrayViewModel(IRedType data, RDTDataViewModel tab) : base(data, tab)
    {
        Init();
    }

    public CArrayViewModel(IRedType export, ReferenceSocket socket) : base(export, socket)
    {
        Init();
    }

    private void Init()
    {
        ShouldShowArrayOps = true;

        this.PropertyChanged += delegate(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Parent))
            {
                DuplicateChunkCommand.NotifyCanExecuteChanged();
            }
        };
    }

    private bool CanDuplicateChunk() => Parent is not null;
    [RelayCommand(CanExecute = nameof(CanDuplicateChunk))]
    private void DuplicateChunk()
    {
        if (Parent is null)
        {
            return;
        }

        if (Data is IRedCloneable irc)
        {
            Parent.InsertChild(Parent.GetIndexOf(this) + 1, (IRedType)irc.DeepCopy());
        }
        else if (Data is not null)
        {
            Parent.InsertChild(Parent.GetIndexOf(this) + 1, Data);
        }
    }
}