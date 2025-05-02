using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon.Pun;

public class SceneController : MonoBehaviourPunCallbacks
{
    [Header("Configuration")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string menuSceneName = "Lobby";
    [SerializeField] private string powerupScreen = "PowerSelect";
    [SerializeField] private float sceneTransitionDelay = 0.5f;

    [Header("Events")]
    public UnityEvent OnSceneTransitionStart;
    public UnityEvent OnSceneTransitionComplete;

    private bool _isTransitioning = false;

    // Singleton pattern for easy access
    private static SceneController _instance;
    public static SceneController Instance => _instance;

    private PhotonView _photonView;

    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (!GetComponent<PhotonView>())
        {
            PhotonView newView = gameObject.AddComponent<PhotonView>();
            newView.ViewID = 999; // Assign unique reserved ID
            newView.OwnershipTransfer = OwnershipOption.Fixed;
        }
    }

    public void LoadGameScene()
    {
        if (_isTransitioning) return;
        
        StartCoroutine(LoadSceneAsync(gameSceneName));
    }

    public void LoadMenuScene()
    {
        if (_isTransitioning) return;
        
        StartCoroutine(LoadSceneAsync(menuSceneName));
    }

    public void LoadPowerSelectScene()
    {
        if (_isTransitioning) return;

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Master client loads the scene for everyone
                PhotonNetwork.LoadLevel(powerupScreen);
            }
        }
        else
        {
            // Single player fallback
            StartCoroutine(LoadSceneAsync(powerupScreen));
        }
    }

    public void ReloadCurrentScene()
    {
        if (_isTransitioning) return;
        
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().name));
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        _isTransitioning = true;
        
        // Trigger transition start events
        OnSceneTransitionStart?.Invoke();

        // Optional: Add fade-out animation here
        yield return new WaitForSeconds(sceneTransitionDelay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Loading progress (0.0 - 0.9)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // When loading is almost complete (0.9), wait for final delay
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(sceneTransitionDelay);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Trigger transition complete events
        OnSceneTransitionComplete?.Invoke();
        _isTransitioning = false;
    }

    // For button click events
    public void OnUIButtonPressed()
    {
        LoadGameScene();
    }
}