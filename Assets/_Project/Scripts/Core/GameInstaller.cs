using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-10000)]
public class GameInstaller : InstallerBehaviourBase {
  /*[SerializeField] private RootPresenterBase _rootUI;
  [SerializeField] private GlobalAudioSettings _audioSettings;
  [SerializeField] private List<AudioEventsCollection> _audioCollections;*/

  protected override void Install() {
    /*// Core services
    BindAsSingleton(new SaveGameManager(new PlayerPrefsData(), true));
    BindAsSingleton(new StateService(true));
    BindAsSingleton(new Ticker());

    // UI
    BindAsSingleton(new ViewService(_rootUI, true));

    // Audio
    BindAsSingleton(new SoundService(_audioSettings, _audioCollections));

    // Models
    var modelService = new ModelService(new PlayerPrefsBridge<PlayerPrefsData>(), true);
    BindAsSingleton(modelService);*/

    BindAsSingleton(new PlayerAccessor());
  }
}
