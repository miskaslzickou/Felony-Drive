using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;

public class MenuButton
{
    private Button button;
   
    public MenuButton(string elementName, UIDocument uiDocument, Action onClickFunc)
    {
        var root = uiDocument.rootVisualElement;
        button = root.Q<Button>(elementName);
        button.clicked += onClickFunc;
        

        button.RegisterCallback<PointerEnterEvent>(e => button.style.scale=new StyleScale(new Vector2(1.2f,1.2f)));
        button.RegisterCallback<PointerLeaveEvent>(e => button.style.scale = new StyleScale(new Vector2(1f, 1f)));
    }
}

public class MainMenu : MonoBehaviour
{
    
    public UIDocument uiDocument;

    public Settings settings;
    void OnEnable()
    {
        new MenuButton("PlayButton", uiDocument, Play);
        new MenuButton("SettingsButton", uiDocument,settings.SettingsToggle);
        new MenuButton("QuitButton", uiDocument, Quit);
    }
    
    public void Play() => SceneManager.LoadScene("Game");
    public void Quit() => Application.Quit();
 


}