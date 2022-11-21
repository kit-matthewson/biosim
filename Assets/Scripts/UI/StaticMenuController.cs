using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene controller that exists across scene changes so that persistent data can be stored.
/// </summary>
public class StaticMenuController : MonoBehaviour {

    public EvolutionConfig EvolutionConfig;

    private string _last;

    [PublicAPI]
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        EvolutionConfig = ScriptableObject.CreateInstance<EvolutionConfig>();
    }

    // ReSharper disable once ParameterHidesMember
    /// <summary>
    /// Loads scene <c>name</c>.
    /// </summary>
    /// <param name="name">String name of the Scene to load.</param>
    public void LoadScene(string name) {
        _last = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Returns to the previous scene.
    /// </summary>
    public void Back() {
        SceneManager.LoadScene(_last);
    }
}