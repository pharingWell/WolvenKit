using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Splat;
using WolvenKit.Common.Services;
using WolvenKit.Core.Interfaces;
using WolvenKit.Functionality.Services;
using WolvenKit.Models;
using WolvenKit.RED4.Types;
using WolvenKit.ViewModels.Shell;
using static WolvenKit.Modkit.RED4.Serialization.Serialization;
using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.ViewModels.Tools;

public partial class SaveGameBrowserViewModel : ToolViewModel
{
    #region fields

    /// <summary>
    /// Identifies the <see ref="ContentId"/> of this tool window.
    /// </summary>
    public const string ToolContentId = "SaveGameBrowser_Tool";

    /// <summary>
    /// Identifies the caption string used for this tool window.
    /// </summary>
    public const string ToolTitle = "SaveGame Browser";

    private readonly ILoggerService _loggerService;
    private readonly ISettingsManager _settingsManager;
    public AppViewModel MainViewModel => Locator.Current.GetService<AppViewModel>();

    #endregion

    #region constructors

    public SaveGameBrowserViewModel(ILoggerService loggerService, 
        ISettingsManager settingsManager) : base(ToolTitle)
    {
        _loggerService = loggerService;
        _settingsManager = settingsManager;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void OpenSave(string name)
    {
        var fileModel = new FileModel(Path.Combine(ISettingsManager.GetSaveGameDir(), name, "sav.dat"), null);

        MainViewModel.OpenFileCommand.Execute(fileModel).Subscribe();
    }

    #endregion

    public ObservableCollection<SaveRecord> SaveRecords { get; set; } = new();

    public void SetupSaveGames()
    {
        SaveRecords.Clear();

        var di = new DirectoryInfo(ISettingsManager.GetSaveGameDir());
        var saveGames = di.EnumerateDirectories().OrderByDescending(x => x.CreationTime);

        foreach (var saveGameDir in saveGames)
        {
            SaveRecord record = new();
            if (File.Exists(Path.Combine(saveGameDir.FullName, "metadata.9.json")))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(saveGameDir.FullName, "metadata.9.json")))["Data"]["metadata"]!;
                record = SaveRecord.FromNode(node);
            }

            record.Image = new BitmapImage();

            record.Image.BeginInit();
            record.Image.UriSource = new Uri(Path.Combine(saveGameDir.FullName, "screenshot.png"));
            record.Image.DecodePixelWidth = 150;
            record.Image.EndInit();
            
            SaveRecords.Add(record);
        }
    }

    public class SaveRecord
    {
        public BitmapImage Image { get; set; }

        public string Name { get; set; }
        public string MainQuest { get; set; }
        public string Location { get; set; }
        public string TimestampString { get; set; }
        public saveGameMetadata SaveGameMetadata { get; set; } = new();

        public string TimeString => GetTimeString();

        private string GetTimeString()
        {
            var ts = TimeSpan.FromSeconds(SaveGameMetadata.PlaythroughTime);

            return $"{Math.Truncate(ts.TotalHours)}h {ts.Minutes}m";
        }

        public static SaveRecord FromNode(JsonNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var record = new SaveRecord();

            record.Name = node["name"]?.GetValue<string>();
            record.TimestampString = node["timestampString"]?.GetValue<string>();

            record.SaveGameMetadata.GameDefinition = node["gameDefinition"]!.GetValue<string>();
            record.SaveGameMetadata.ActiveQuests = node["activeQuests"]!.GetValue<string>();
            record.SaveGameMetadata.TrackedQuestEntry = node["trackedQuestEntry"]!.GetValue<string>();
            record.SaveGameMetadata.TrackedQuest = node["trackedQuest"]!.GetValue<string>();
            record.SaveGameMetadata.MainQuest = node["mainQuest"]!.GetValue<string>();
            record.SaveGameMetadata.DebugString = node["debugString"]!.GetValue<string>();
            record.SaveGameMetadata.LocationName = node["locationName"]!.GetValue<string>();
            record.SaveGameMetadata.PlayerPosition.X = node["playerPosition"]!["X"]!.GetValue<float>();
            record.SaveGameMetadata.PlayerPosition.Y = node["playerPosition"]!["Y"]!.GetValue<float>();
            record.SaveGameMetadata.PlayerPosition.Z = node["playerPosition"]!["Z"]!.GetValue<float>();
            record.SaveGameMetadata.PlayTime = node["playTime"]!.GetValue<double>();
            record.SaveGameMetadata.PlaythroughTime = node["playthroughTime"]!.GetValue<double>();
            record.SaveGameMetadata.NextSavableEntityID = node["nextSavableEntityID"]!.GetValue<uint>();
            record.SaveGameMetadata.NextNonSavableEntityID = node["nextNonSavableEntityID"]!.GetValue<uint>();
            record.SaveGameMetadata.LifePath = Enum.Parse<gamedataLifePath>(node["lifePath"]!.GetValue<string>());
            record.SaveGameMetadata.BodyGender = node["bodyGender"]!.GetValue<string>();
            record.SaveGameMetadata.BrainGender = node["brainGender"]!.GetValue<string>();
            record.SaveGameMetadata.Level = node["level"]!.GetValue<float>();
            record.SaveGameMetadata.StreetCred = node["streetCred"]!.GetValue<float>();
            record.SaveGameMetadata.Gunslinger = node["gunslinger"]!.GetValue<float>();
            record.SaveGameMetadata.Assault = node["assault"]!.GetValue<float>();
            record.SaveGameMetadata.Demolition = node["demolition"]!.GetValue<float>();
            record.SaveGameMetadata.Athletics = node["athletics"]!.GetValue<float>();
            record.SaveGameMetadata.Brawling = node["brawling"]!.GetValue<float>();
            record.SaveGameMetadata.ColdBlood = node["coldBlood"]!.GetValue<float>();
            record.SaveGameMetadata.Stealth = node["stealth"]!.GetValue<float>();
            record.SaveGameMetadata.Engineering = node["engineering"]!.GetValue<float>();
            record.SaveGameMetadata.Crafting = node["crafting"]!.GetValue<float>();
            record.SaveGameMetadata.Hacking = node["hacking"]!.GetValue<float>();
            record.SaveGameMetadata.CombatHacking = node["combatHacking"]!.GetValue<float>();
            record.SaveGameMetadata.Strength = node["strength"]!.GetValue<float>();
            record.SaveGameMetadata.Intelligence = node["intelligence"]!.GetValue<float>();
            record.SaveGameMetadata.Reflexes = node["reflexes"]!.GetValue<float>();
            record.SaveGameMetadata.TechnicalAbility = node["technicalAbility"]!.GetValue<float>();
            record.SaveGameMetadata.Cool = node["cool"]!.GetValue<float>();
            record.SaveGameMetadata.InitialBuildID = node["initialBuildID"]!.GetValue<string>();
            record.SaveGameMetadata.FinishedQuests = node["finishedQuests"]!.GetValue<string>();
            record.SaveGameMetadata.PlaythroughID = node["playthroughID"]!.GetValue<string>();
            record.SaveGameMetadata.PointOfNoReturnId = node["pointOfNoReturnId"]!.GetValue<string>();
            record.SaveGameMetadata.VisitID = node["visitID"]!.GetValue<string>();
            record.SaveGameMetadata.BuildSKU = node["buildSKU"]!.GetValue<string>();
            record.SaveGameMetadata.BuildPatch = node["buildPatch"]!.GetValue<string>();
            record.SaveGameMetadata.Difficulty = Enum.Parse<gameDifficulty>(node["difficulty"]!.GetValue<string>());

            record.MainQuest = record.SaveGameMetadata.MainQuest;
            record.Location = record.SaveGameMetadata.LocationName;

            var locKeyService = Locator.Current.GetService<LocKeyService>();
            if (locKeyService != null)
            {
                if (ulong.TryParse(record.MainQuest[7..], out var locKey1) && locKeyService.GetEntry(locKey1) != null)
                {
                    record.MainQuest = locKeyService.GetEntry(locKey1).FemaleVariant;
                }

                if (ulong.TryParse(record.Location[7..], out var locKey2) && locKeyService.GetEntry(locKey2) != null)
                {
                    record.Location = locKeyService.GetEntry(locKey2).FemaleVariant;
                }
            }

            return record;
        }
    }
}