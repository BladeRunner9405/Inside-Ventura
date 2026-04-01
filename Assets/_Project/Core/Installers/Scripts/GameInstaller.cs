using System.Collections.Generic;
using UnityEngine;
using CherryFramework.DependencyManager;
using CherryFramework.UI.Views;
using CherryFramework.SoundService;
using CherryFramework.SaveGameManager;
using CherryFramework.StateService;
using CherryFramework.TickDispatcher;
using CherryFramework.DataModels;
using CherryFramework.DataModels.ModelDataStorageBridges;
using CherryFramework.Utils.PlayerPrefsWrapper;

[DefaultExecutionOrder(-10000)]
public class GameInstaller : InstallerBehaviourBase 
{
    [Header("UI Settings")]
    [SerializeField] private RootPresenterBase _rootUI;
    
    // ДОБАВИЛИ ПРЯМУЮ ССЫЛКУ:
    [SerializeField] private DragAndDropManager _dragDropManager;

    [Header("Audio Settings")]
    [SerializeField] private GlobalAudioSettings _audioSettings;
    [SerializeField] private List<AudioEventsCollection> _audioCollections;

    protected override void Install() 
    {
        // 1. Core Services
        BindAsSingleton(new SaveGameManager(new PlayerPrefsData(), true));
        BindAsSingleton(new StateService(true));
        BindAsSingleton(new Ticker());

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