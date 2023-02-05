﻿using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WolvenKit.App.Helpers;
using WolvenKit.App.Models.Nodify;
using WolvenKit.App.ViewModels.Documents;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Common.Extensions;
using WolvenKit.Core.Extensions;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.Models;

public partial class CNameWrapper : ObservableObject, Nodify.INode<ReferenceSocket>
{
    //public CName CName => Socket.File;

    [ObservableProperty]
    private System.Windows.Point _location;

    public double Width { get; set; }

    public double Height { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenRefCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadRefCommand))]
    private ReferenceSocket _socket;

    public IList<ReferenceSocket> Inputs
    {
        get => new List<ReferenceSocket>(new ReferenceSocket[] { Socket });
        set
        {

        }
    }

    public IList<ReferenceSocket> Outputs { get; set; } = new List<ReferenceSocket>();

    public RDTDataViewModel DataViewModel { get; set; }

    public CNameWrapper(RDTDataViewModel vm, ReferenceSocket socket)
    {
        DataViewModel = vm;
        _socket = socket;
    }

    private bool CanOpenRef() => !CName.IsNullOrEmpty(Socket.File) && DataViewModel.Parent.RelativePath != Socket.File;
    [RelayCommand(CanExecute = nameof(CanOpenRef))]
    private void OpenRef()
    {
        IocHelper.GetService<AppViewModel>().OpenFileFromDepotPath(Socket.File.ToString().NotNull());
    }

    private bool CanLoadRef() => Socket.File != CName.Empty;
    [RelayCommand(CanExecute = nameof(CanLoadRef))]
    private void LoadRef()
    {
        var cr2w = DataViewModel.Parent.GetFileFromDepotPathOrCache(Socket.File);
        if (cr2w != null && cr2w.RootChunk != null)
        {
            var chunk = ChunkViewModel.Create(cr2w.RootChunk, Socket);
            chunk.Location = Location;

            DataViewModel.Nodes.Remove(this);
            DataViewModel.Nodes.Add(chunk);
            DataViewModel.LookForReferences(chunk);
        }
    }
}
