//Michal Mikuš, 3C, PVA, Felony Drive
using UnityEngine;

public class Pause : MonoBehaviour
{
    private PlayerActions playerActions;
    public bool isPaused = false;
    public Settings settings;
    void OnEnable() => playerActions.Menu.Enable();
    void OnDisable() => playerActions.Menu.Disable();

    void Awake()
    {
       playerActions= new PlayerActions();
        playerActions.Menu.Pause.performed += ctx =>
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            settings.SettingsToggle();
            AudioListener.pause = isPaused;
        };
    }
   


}
