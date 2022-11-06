using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticMenuController : MonoBehaviour {

    private string last;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string name) {
        last = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }

    public void Back() {
        SceneManager.LoadScene(last);
    }
}