using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;
    
    // Manage the current state of the screen resolution and fullscreen mode
    [HideInInspector] public bool isFullScreen = false;
    [HideInInspector] public int BaseResolutionX = 1280;
    [HideInInspector] public int BaseResolutionY = 720;
    private bool active = false;
    
    private bool applyFullscreenAfterLoad = false;
    
    public enum PostLoadAction { None, NewGame, LoadGame }
    private PostLoadAction pendingAction = PostLoadAction.None;
    
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    IEnumerator SetResolution(int resolution)
    {
        active = true;
        switch (resolution)
        {
            case 0: // Windowed 1280x720
                Screen.SetResolution(BaseResolutionX, BaseResolutionY, false);
                isFullScreen = false;
                break;
            case 1: // Fullscreen
                Resolution res = Screen.currentResolution;
                Screen.SetResolution(res.width, res.height, true);
                BaseResolutionX = res.width;
                BaseResolutionY = res.height;
                isFullScreen = true;
                break;
        }
        active = false;
        yield return null;
    }
    
    public void ChangeResolution(int resolution)
    {
        if (!active)
        {
            StartCoroutine(SetResolution(resolution));
        }
    }
    
    public void ChangeScene(string sceneName, PostLoadAction action)
    {
        applyFullscreenAfterLoad = isFullScreen;
        ChangeResolution(0);
        
        pendingAction = action;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    
    public void UnloadGameScene()
    {
        var audioManager = AudioManager.instance;
        audioManager.StopSoundEffect();

        // Unload la scena di gi mkioco (assumendo build index 1)
        SceneManager.UnloadSceneAsync(1).completed += (op) =>
        {
            // Una volta che la scena 1 è stata scaricata, riattivo il Canvas della scena 0
            Scene scene0 = SceneManager.GetSceneByBuildIndex(0);
            if (scene0.isLoaded)
            {
                foreach (var root in scene0.GetRootGameObjects())
                {
                    if (root.name == "Canvas")
                    {
                        root.SetActive(true); // riattiva il Canvas
                    }
                    if (root.name == "EventSystem")
                    {
                        root.SetActive(true); // riattiva l'EventSystem
                    }
                }
            }
        };
    }

    public void StartNewGame()
    {
        // Controlla se il file di salvataggio esiste, se c'é avvisa l'utente che il gioco verrà resettato
        string savepath = Application.persistentDataPath + "/savegame.json";
        if (System.IO.File.Exists(savepath))
        {
            // Attiva il gameobject di richiesta di conferma
            var WarningPanel = GameObject.Find("Canvas").transform.Find("WarningPanel").gameObject;
            WarningPanel.SetActive(true);
            return;
        }
        var audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.StartSoundEffect();
        ChangeScene("GameScene", PostLoadAction.NewGame);
    }

    public void StartNewGameWithConfirm()
    {
        // Cancella il file di salvataggio se esiste
        string savepath = Application.persistentDataPath + "/savegame.json";
        if (System.IO.File.Exists(savepath))
        {
            try
            {
                System.IO.File.Delete(savepath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error deleting save file: " + e.Message);
                // Mostra un messaggio di errore all'utente
                return;
            }
        }
        var audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.StartSoundEffect();
        ChangeScene("GameScene", PostLoadAction.NewGame);
    }
    
    public void LoadGame()
    {
        string savepath = Application.persistentDataPath + "/savegame.json";
        if (!System.IO.File.Exists(savepath))
        {
            // Attiva il gameobject di richiesta di conferma
            var ErrorPanel = GameObject.Find("Canvas").transform.Find("ErrorPanel").gameObject;
            ErrorPanel.SetActive(true);
            return;
        }
        var audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.StartSoundEffect();
        ChangeScene("GameScene", PostLoadAction.LoadGame);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (applyFullscreenAfterLoad)
        {
            ChangeResolution(1);
            applyFullscreenAfterLoad = false;
        }

        // Disattivo l'EventSystem e il Canvas della scena precedente (se presente)
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            var oldScene = SceneManager.GetSceneAt(i);
            if (oldScene != scene && oldScene.isLoaded)
            {
                foreach (var root in oldScene.GetRootGameObjects())
                {
                    // Disattivo Canvas
                    if (root.name == "Canvas")
                        root.SetActive(false);

                    // Disattivo EventSystem se presente
                    var es = root.GetComponentInChildren<EventSystem>();
                    if (es != null)
                        es.gameObject.SetActive(false);
                }
            }
        }
        
        // Eseguo l'azione post caricamento scena
        // Scorro i root objects della scena appena caricata
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "Canvas")
            {
                // Cerco AziendaUnity nei figli del Canvas
                var azienda = root.GetComponentInChildren<Azienda>();
                if (azienda != null)
                {
                    if (pendingAction == PostLoadAction.NewGame)
                        azienda.OnNewGame();
                    else if (pendingAction == PostLoadAction.LoadGame)
                        azienda.OnLoadGame();
                }
                break; // trovato il Canvas, esco dal ciclo
            }
        }

        pendingAction = PostLoadAction.None;
    }
}
