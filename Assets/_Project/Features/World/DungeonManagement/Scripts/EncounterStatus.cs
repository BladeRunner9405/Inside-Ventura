using UnityEngine;

namespace InsideVentura.World {
  public class EncounterStatus {
    private static bool _active = false;

    public static bool Active {
      get => _active;
      set {
        Debug.Log("Encounter status update: " + value);
        _active = value;
      }
    }
  }
}
