using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.DefaultInputActions;
public class Settings : MonoBehaviour
{
    public PlayerActions playerActions;
    public UIDocument UIDocument;
    public UIDocument MainMenuUI;
    public AudioMixer audioMixer;
    public bool settingsOpen;
    private bool initialized = false;
    public class MenuButton
    {
        private Button button;

        public MenuButton(string elementName, UIDocument uiDocument, Action onClickFunc)
        {
            var root = uiDocument.rootVisualElement;
            button = root.Q<Button>(elementName);
            button.clicked += onClickFunc;


            button.RegisterCallback<PointerEnterEvent>(e => button.style.scale = new StyleScale(new Vector2(1.2f, 1.2f)));
            button.RegisterCallback<PointerLeaveEvent>(e => button.style.scale = new StyleScale(new Vector2(1f, 1f)));
        }
    }

    void Start()
    {
        new MenuButton("QuitMenuButton", UIDocument, () => {
            SaveData.SaveGameplayData();
            SceneManager.LoadScene("MainMenu");
            
        
        });
        new MenuButton("QuitDesktopButton", UIDocument, () => Application.Quit());
        UIDocument.rootVisualElement.style.display = DisplayStyle.None;
        if (initialized) return;
        initialized = true;
        playerActions = new PlayerActions();
        SetupGraphics();
        SetupAudio();
        SetupKeybinds();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void SetupGraphics()
    {
        if (PlayerPrefs.HasKey("quality"))
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));
        if (PlayerPrefs.HasKey("vsync"))
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync");
        if (PlayerPrefs.HasKey("screenMode"))
            Screen.fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt("screenMode");
        if (PlayerPrefs.HasKey("resolution"))
        {
            var parts = PlayerPrefs.GetString("resolution").Split('x');
            Screen.SetResolution(int.Parse(parts[0]), int.Parse(parts[1]), Screen.fullScreen);
        }
        var root = UIDocument.rootVisualElement;
        var dropdownRes = root.Q<DropdownField>("ResolutionDropdown");
        dropdownRes.choices = Screen.resolutions
            .Select(r => $"{r.width}x{r.height}")
            .Distinct()
            .ToList();

        dropdownRes.index = dropdownRes.choices.IndexOf($"{Screen.currentResolution.width}x{Screen.currentResolution.height}");
        dropdownRes.RegisterValueChangedCallback(evt =>
        {
            var parts = evt.newValue.Split('x');
            Screen.SetResolution(int.Parse(parts[0]), int.Parse(parts[1]), Screen.fullScreen);
            PlayerPrefs.SetString("resolution", evt.newValue);
        });
        var dropdownScreen = root.Q<DropdownField>("ScreenDropdown");
        dropdownScreen.choices = new List<string> { "FULLSCREEN", "BORDERLESS", "WINDOWED" };
        dropdownScreen.RegisterValueChangedCallback(evt =>
        {
            switch (evt.newValue)
            {
                case "FULLSCREEN": Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
                case "BORDERLESS": Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
                case "WINDOWED": Screen.fullScreenMode = FullScreenMode.Windowed; break;
            }
            PlayerPrefs.SetInt("screenMode", (int)Screen.fullScreenMode);
        });

        var dropDownQuality = root.Q<DropdownField>("QualityDropdown");
        dropDownQuality.choices = QualitySettings.names.ToList();
        dropDownQuality.index = QualitySettings.GetQualityLevel();

        dropDownQuality.RegisterValueChangedCallback(evt =>
        {
            QualitySettings.SetQualityLevel(dropDownQuality.index, true);
            PlayerPrefs.SetInt("quality", dropDownQuality.index);
        });
        if (PlayerPrefs.HasKey("vsync"))
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync");
        var toggleVsync = root.Q<Toggle>("VSYNC");
        toggleVsync.value = QualitySettings.vSyncCount == 1;
        toggleVsync.RegisterValueChangedCallback(evt => {

            QualitySettings.vSyncCount = evt.newValue ? 1 : 0;
            PlayerPrefs.SetInt("vsync", QualitySettings.vSyncCount);
        });

    }
    public void SettingsToggle()
    {
        settingsOpen = !settingsOpen;
        if (MainMenuUI != null)
        {
            MainMenuUI.rootVisualElement.style.display = settingsOpen ? DisplayStyle.None : DisplayStyle.Flex;
        }
        UIDocument.rootVisualElement.style.display = settingsOpen ?  DisplayStyle.Flex :DisplayStyle.None;


    }
    void SetupAudio()
    {
        var root = UIDocument.rootVisualElement;
        var masterSlider = root.Q<Slider>("Master");
        var musicSlider = root.Q<Slider>("Music");
        var sfxSlider = root.Q<Slider>("SFX");

        // načtení
        masterSlider.value = PlayerPrefs.GetFloat("master", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfx", 1f);

        audioMixer.SetFloat("MasterVol", Mathf.Log10(masterSlider.value) * 20);
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sfxSlider.value) * 20);

        // callbacky
        masterSlider.RegisterValueChangedCallback(evt =>
        {
            audioMixer.SetFloat("MasterVol", Mathf.Log10(evt.newValue) * 20);
            PlayerPrefs.SetFloat("master", evt.newValue);
        });
        musicSlider.RegisterValueChangedCallback(evt =>
        {
            audioMixer.SetFloat("MusicVol", Mathf.Log10(evt.newValue) * 20);
            PlayerPrefs.SetFloat("music", evt.newValue);
        });
        sfxSlider.RegisterValueChangedCallback(evt =>
        {
            audioMixer.SetFloat("SFXVol", Mathf.Log10(evt.newValue) * 20);
            PlayerPrefs.SetFloat("sfx", evt.newValue);
        });

    }
    void SetupKeybinds()
    {
        if (PlayerPrefs.HasKey("keybinds"))
            playerActions.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("keybinds"));
        var root = UIDocument.rootVisualElement;
        var keysParent = root.Q<VisualElement>("KeysParent");
      
        var actionMaps = playerActions.asset.actionMaps;
        foreach (var actionMap in actionMaps)
        {
           
            foreach (var action in actionMap)
            {
                var label = new Label(action.name);
                label.style.width = 320;
               
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.Add(label);


                for (int i = action.bindings.Count() - 1; i >= 0; i--)
                {
                    if (action.bindings[i].isComposite) continue;
                    var btn = new Button();
                    btn.text = action.bindings[i].ToDisplayString();
                    btn.style.backgroundColor = Color.white;
                    btn.style.color = Color.black;
                    //i se mění je potřeba uložit
                    int index = i;
                    btn.clicked += () =>
                    {
                        action.Disable();
                        action.PerformInteractiveRebinding(index)
                            .OnComplete(op => { op.Dispose();
                                btn.text = action.GetBindingDisplayString(index);
                                PlayerPrefs.SetString("keybinds", playerActions.asset.SaveBindingOverridesAsJson());
                            })
                                
                            .Start();
                    };
                    btn.style.width = 200;
                    
                    
                    row.Add(btn);
                   

                }
                keysParent.Add(row);

            }
            
        }
       

    }

    void OnDisable()
    {
        playerActions.Disable();
    }
    


    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0 && SceneManager.GetActiveScene().name == "MainMenu")
        {
            Time.timeScale = 1;
            AudioListener.pause = false;

        }

    }
}
