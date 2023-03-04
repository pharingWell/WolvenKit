﻿using System;
using WolvenKit.App.Controllers;
using WolvenKit.App.Models.Docking;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.App.ViewModels.Exporters;
using WolvenKit.App.ViewModels.Importers;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.App.ViewModels.Tools;
using WolvenKit.Common;
using WolvenKit.Common.Interfaces;
using WolvenKit.Common.Services;
using WolvenKit.Core.Interfaces;
using WolvenKit.Core.Services;
using WolvenKit.Modkit.RED4.Tools;
using WolvenKit.RED4.CR2W;

namespace WolvenKit.App.Factories;

public class PaneViewModelFactory : IPaneViewModelFactory
{
    private readonly IChunkViewmodelFactory _chunkViewmodelFactory;

    private readonly ILoggerService _loggerService;
    private readonly IProjectManager _projectManager;
    private readonly IWatcherService _watcherService;
    private readonly IModTools _modTools;
    private readonly IProgressService<double> _progressService;
    private readonly IGameControllerFactory _gameController;
    private readonly IPluginService _pluginService;
    private readonly ISettingsManager _settingsManager;
    private readonly INotificationService _notificationService;
    private readonly IArchiveManager _archiveManager;
    private readonly MeshTools _meshTools;
    private readonly Red4ParserService _parserService;
    private readonly ITweakDBService _tweakDbService;
    private readonly ILocKeyService _locKeyService;
    private readonly IHashService _hashService;

    private readonly PropertiesViewModel _propertiesViewModel;

    public PaneViewModelFactory(
        IProjectManager projectManager,
        ILoggerService loggerService,
        IWatcherService watcherService,
        IProgressService<double> progressService,
        IModTools modTools,
        IGameControllerFactory gameController,
        IPluginService pluginService,
        ISettingsManager settingsManager,
        INotificationService notificationService,
        IArchiveManager archiveManager,
        IChunkViewmodelFactory chunkViewmodelFactory,
        MeshTools meshTools,
        Red4ParserService parserService,
        ITweakDBService tweakDbService,
        ILocKeyService locKeyService,
        IHashService hashService,
        PropertiesViewModel propertiesViewModel
        )
    {
        _projectManager = projectManager;
        _loggerService = loggerService;
        _watcherService = watcherService;
        _modTools = modTools;
        _progressService = progressService;
        _gameController = gameController;
        _pluginService = pluginService;
        _settingsManager = settingsManager;
        _notificationService = notificationService;
        _archiveManager = archiveManager;
        _chunkViewmodelFactory = chunkViewmodelFactory;
        _meshTools = meshTools;
        _parserService = parserService;
        _tweakDbService = tweakDbService;
        _locKeyService = locKeyService;
        _hashService = hashService;
        _propertiesViewModel = propertiesViewModel;
    }

    public LogViewModel LogViewModel() => new(_loggerService);
    public ProjectExplorerViewModel ProjectExplorerViewModel(AppViewModel appViewModel)
        => new(appViewModel, _projectManager, _loggerService, _watcherService, _progressService, _modTools, _gameController, _pluginService, _settingsManager);
    public PropertiesViewModel PropertiesViewModel()
        => _propertiesViewModel;
    public AssetBrowserViewModel AssetBrowserViewModel(AppViewModel appViewModel)
        => new(appViewModel, _projectManager, _notificationService, _gameController, _archiveManager, _settingsManager, _progressService, _loggerService, _pluginService, _watcherService);
    public TweakBrowserViewModel TweakBrowserViewModel(AppViewModel appViewModel) 
        => new(appViewModel, _chunkViewmodelFactory, _settingsManager, _notificationService, _projectManager, _loggerService, _tweakDbService, _locKeyService);
    public LocKeyBrowserViewModel LocKeyBrowserViewModel() => new(_projectManager, _loggerService, _watcherService, _progressService, _modTools, _gameController, _archiveManager, _locKeyService);
    
    public TextureImportViewModel TextureImportViewModel() => new(_gameController, _settingsManager, _watcherService, _loggerService, _projectManager, _notificationService, _archiveManager, _pluginService, _hashService, _modTools, _parserService, _progressService);
    public TextureExportViewModel TextureExportViewModel() => new(_gameController, _settingsManager, _watcherService, _loggerService, _projectManager, _notificationService, _archiveManager, _pluginService, _hashService, _modTools, _parserService, _progressService);
}
