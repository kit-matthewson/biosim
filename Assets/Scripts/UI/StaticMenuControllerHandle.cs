using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Provides an interface to a <c>StaticMenuController</c>, allowing the static controller to exist between scenes.
/// </summary>
public class StaticMenuControllerHandle : MonoBehaviour {
    /// <summary>
    /// Name of the <c>GameObject</c> with a <c>StaticMenuController</c> that this script should use.
    /// </summary>
    public string StaticObjectName = "Static";

    public StaticMenuController StaticController { get; private set; }

    [PublicAPI]
    private void Awake() {
        StaticController = GameObject.Find(StaticObjectName).GetComponent<StaticMenuController>();
    }

    // ReSharper disable once ParameterHidesMember
    /// <summary>
    /// Loads scene <c>name</c>.
    /// </summary>
    /// <param name="name">String name of the Scene to load.</param>
    public void LoadScene(string name) {
        StaticController.LoadScene(name);
    }

    /// <summary>
    /// Return to the previous scene.
    /// </summary>
    public void Back() {
        StaticController.Back();
    }
}