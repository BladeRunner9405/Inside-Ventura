using System.Collections.Generic;
using CherryFramework.DataModels;
using CherryFramework.DataModels.ModelDataStorageBridges;
using CherryFramework.DependencyManager;
using CherryFramework.SaveGameManager;
using CherryFramework.SoundService;
using CherryFramework.StateService;
using CherryFramework.TickDispatcher;
using CherryFramework.UI.Views;
using CherryFramework.Utils.PlayerPrefsWrapper;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class GameInstaller : InstallerBehaviourBase
{
  [Header("UI Settings")]
  [SerializeField]
  private RootPresenterBase _rootUI;

  // ДОБАВИЛИ ПРЯМУЮ ССЫЛКУ:
  [SerializeField]
  private DragAndDropManager _dragDropManager;

  [Header("Audio Settings")]
  [SerializeField]
  private GlobalAudioSettings _audioSettings;

  [SerializeField]
  private List<AudioEventsCollection> _audioCollections;

  [Header("World Settings")]
  [SerializeField]
  private InsideVentura.World.DungeonManager _dungeonManager;

  protected override void Install()
  {
    // 1. Core Services
    Ticker _ticker = new Ticker();
    BindAsSingleton(_ticker);
    BindAsSingleton(new SaveGameManager(new PlayerPrefsData(), true));
    BindAsSingleton(new StateService(_ticker, true));

    //

    // 2. Audio
    if (_audioSettings != null)
    {
      BindAsSingleton(new SoundService(_audioSettings, _audioCollections));
    }

    // 3. Models
    var modelService = new ModelService(new PlayerPrefsBridge<PlayerPrefsData>(), true);
    BindAsSingleton(modelService);

    // 4. UI Framework
    if (_rootUI)
    {
      BindAsSingleton(new ViewService(_rootUI, true));
    }

    // 5. Project Specific
    BindAsSingleton(new PlayerAccessor());

    // РЕГИСТРАЦИЯ DUNGEON MANAGER:
    if (_dungeonManager != null)
    {
      BindAsSingleton(_dungeonManager);
    }
    else
    {
      Debug.LogError("[GameInstaller] ОШИБКА: DungeonManager не назначен в инспекторе!");
    }

    // 6. 100% надежная регистрация Drag & Drop
    if (_dragDropManager != null)
    {
      BindAsSingleton(_dragDropManager);
    }
    else
    {
      Debug.LogError("[GameInstaller] ОШИБКА: DragAndDropManager не назначен в инспекторе!");
    }
  }
}
