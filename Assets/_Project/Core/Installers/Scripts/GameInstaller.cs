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
using InsideVentura.World;
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
  [SerializeField]
  private ThoughtsCompatibilityManager _thoughtsCompatibilityManager;
  [SerializeField]
  private ThoughtItemTooltip _thoughtItemTooltip;

  [Header("Audio Settings")]
  [SerializeField]
  private GlobalAudioSettings _audioSettings;

  [SerializeField]
  private List<AudioEventsCollection> _audioCollections;

  [Header("World Settings")]
  [SerializeField]
  private DungeonManager _dungeonManager;
  [SerializeField]
  private ThoughtsAvaliabilityManager _thoughtsAvaliabilityManager;

  [Header("Language (Yandex SDK)")]
  [SerializeField]
  private LanguageManager _languageManager;

  protected override void Install()
  {
    // 1. Core Services
    Ticker _ticker = new Ticker();
    BindAsSingleton(_ticker);
    BindAsSingleton(new SaveGameManager(new PlayerPrefsData(), true));
    BindAsSingleton(new StateService(_ticker, true));

    // 1.5. Langauge Manager
    if (_languageManager != null)
    {
      BindAsSingleton(_languageManager);
    }
    else
    {
      Debug.LogWarning("[GameInstaller] ОШИБКА: LanguageManager не назначен в инспекторе!");
    }

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

    if (_thoughtsCompatibilityManager != null)
    {
      BindAsSingleton(_thoughtsCompatibilityManager);
    }
    else
    {
      Debug.LogWarning("[GameInstaller] ПРЕДУПРЕЖДЕНИЕ: ThoughtsCompatibilityManager не назначен в инспекторе!");
    }
    if (_thoughtItemTooltip != null)
    {
      BindAsSingleton(_thoughtItemTooltip);
    }
    else
    {
      Debug.LogWarning("[GameInstaller] ПРЕДУПРЕЖДЕНИЕ: ThoughtItemTooltip не назначен в инспекторе!");
    }

    if (_thoughtsAvaliabilityManager != null)
    {
      BindAsSingleton(_thoughtsAvaliabilityManager);
    }
    else
    {
      Debug.LogWarning("[GameInstaller] ПРЕДУПРЕЖДЕНИЕ: ThoughtsAvaliabilityManager не назначен в инспекторе!");
    }

  }
}
