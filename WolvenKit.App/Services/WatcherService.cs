using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using WolvenKit.App.Models;
using WolvenKit.Core.Extensions;

namespace WolvenKit.App.Services;

/// <summary>
/// This service watches certain locations in the game files and notifies changes
/// </summary>
public class WatcherService : ObservableObject, IWatcherService
{
    #region fields

    private readonly SourceCache<FileModel, ulong> _files = new(_ => _.Hash);
    public IObservableCache<FileModel, ulong> Files => _files;
    public FileCollection NewFiles { get; } = new ();

    private readonly IProjectManager _projectManager;

    private string? _baseDir;

    private readonly ConcurrentQueue<FileSystemEventArgs> _events = new();
    private FileSystemWatcher? _modsWatcher;
    private Thread? _thread;

    public FileModel? LastSelect { get; set; }

    private readonly List<string> _ignoredExtensions = new()
    {
        ".TMP",
        ".PDNSAVE"
    };

    #endregion

    public bool IsSuspended { get; set; }

    public WatcherService(IProjectManager projectManager)
    {
        _projectManager = projectManager;
        _projectManager.PropertyChanged += ProjectManager_PropertyChanged;
    }

    private void ProjectManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName ==  nameof(IProjectManager.IsProjectLoaded))
        {
            if (_projectManager.IsProjectLoaded)
            {
                WatchLocation(_projectManager.ActiveProject.NotNull().ProjectDirectory);
                QueueRefresh();
            }
            else
            {
                UnwatchLocation();
            }
        }
    }

    private void WatchLocation(string location)
    {
        NewFiles.Clear();
        _events.Clear();

        _baseDir = location;

        _modsWatcher = new FileSystemWatcher(_baseDir, "*")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Attributes | NotifyFilters.DirectoryName,
            IncludeSubdirectories = true
        };
        _modsWatcher.Created += OnChanged;
        _modsWatcher.Changed += OnChanged;
        _modsWatcher.Deleted += OnChanged;
        _modsWatcher.Renamed += OnRenamed;

        foreach (var file in Directory.GetFileSystemEntries(_baseDir, "*.*", SearchOption.AllDirectories))
        {
            var directory = Path.GetDirectoryName(file)!;
            var fileName = Path.GetFileName(file);
            _events.Enqueue(new FileSystemEventArgs(WatcherChangeTypes.Created, directory, fileName));
        }

        _modsWatcher.EnableRaisingEvents = true;

        _thread = new Thread(Processor);
        _thread.Start();
    }

    private void UnwatchLocation()
    {
        if (_modsWatcher != null)
        {
            _modsWatcher.EnableRaisingEvents = false;
            _modsWatcher.Created -= OnChanged;
            _modsWatcher.Changed -= OnChanged;
            _modsWatcher.Deleted -= OnChanged;
            _modsWatcher.Renamed -= OnRenamed;
        }
        _thread = null;
        _events.Clear();
    }

    private void ProcessEvent(FileSystemEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(_baseDir);

        var newRelativePath = Path.GetRelativePath(_baseDir, args.FullPath);

        if (args is RenamedEventArgs ren)
        {
            NewFiles.RemoveEntry(Path.GetRelativePath(_baseDir, ren.OldFullPath));
            NewFiles.AddEntry(newRelativePath, _baseDir);
        }
        else
        {
            if (args.ChangeType == WatcherChangeTypes.Created)
            {
                NewFiles.AddEntry(newRelativePath, _baseDir);
            }
            else if (args.ChangeType == WatcherChangeTypes.Deleted)
            {
                NewFiles.RemoveEntry(newRelativePath);
            }
        }
    }

    private void Processor()
    {
        while (true)
        {
            if (IsSuspended || !_events.TryDequeue(out var args))
            {
                Thread.Sleep(50);
                continue;
            }

            try
            {
                ProcessEvent(args);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }

    public void QueueRefresh()
    {

    }

    public FileModel? GetFileModelFromHash(ulong hash) => null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (_projectManager.ActiveProject is null)
        {
            return;
        }

        _events.Enqueue(e);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (_projectManager.ActiveProject is null)
        {
            return;
        }

        _events.Enqueue(e);
    }
}
