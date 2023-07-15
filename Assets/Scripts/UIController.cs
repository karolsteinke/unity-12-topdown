using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text healthLabel;
    [SerializeField] private InventoryPopup popup;

    void Awake() {
        Messenger.AddListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
    }

    void OnDestroy() {
        Messenger.RemoveListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
    }

    void Start() {
        OnHealthUpdated();

        //Initialize pop-up to be hidden at start
        popup.gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            bool isShowing = popup.gameObject.activeSelf;
            popup.gameObject.SetActive(!isShowing);
            popup.Refresh();
        }
    }

    //Event listener calls function to update health label
    private void OnHealthUpdated() {
        string message = "Health: " + Managers.Player.health + "/" + Managers.Player.maxHealth;
        healthLabel.text = message;
    }
}
