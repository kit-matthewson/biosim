using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticMenuController : MonoBehaviour {

    public EvolutionConfig evolutionConfig;

    private string last;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        evolutionConfig = ScriptableObject.CreateInstance<EvolutionConfig>();
    }
    public void LoadScene(string name) {
        last = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }

    public void Back() {
        SceneManager.LoadScene(last);
    }
}