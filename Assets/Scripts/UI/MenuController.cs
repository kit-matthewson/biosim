using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public string staticObjectName = "Static";
    
    private StaticMenuController controller;

    private void Start() {
        controller = GameObject.Find(staticObjectName).GetComponent<StaticMenuController>();
    }

    public void LoadScene(string name) {
        controller.LoadScene(name);
    }

    public void Back() {
        controller.Back();
    }
}