using CherryFramework.DependencyManager;
using CherryFramework.UI.Views;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class GameInstaller : InstallerBehaviourBase {
  [SerializeField] private RootPresenterBase _rootUI;

  /*[SerializeField] private GlobalAudioSettings _audioSettings;
  [SerializeField] private List<AudioEventsCollection> _audioCollections;*/

  protected override void Install() {

    /*// Core services
    BindAsSingleton(new SaveGameManager(new PlayerPrefsData(), true));
    BindAsSingleton(new StateService(true));
    BindAsSingleton(new Ticker());

    // Audio
    BindAsSingleton(new SoundService(_audioSettings, _audioCollections));

    // Models
    var modelService = new ModelService(new PlayerPrefsBridge<PlayerPrefsData>(), true);
    BindAsSingleton(modelService);*/

    // UI
    BindAsSingleton(new ViewService(_rootUI, true));

    BindAsSingleton(new PlayerAccessor());
  }
}
