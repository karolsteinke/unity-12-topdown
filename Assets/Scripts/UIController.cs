using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text healthLabel;
    [SerializeField] private InventoryPopup popup;
    [SerializeField] private Text levelEnding;

    void Awake() {
        Messenger.AddListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETED, OnLevelCompleted);
        Messenger.AddListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvent.GAME_COMPLETE, OnGameComplete);
    }

    void OnDestroy() {
        Messenger.RemoveListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETED, OnLevelCompleted);
        Messenger.RemoveListener(GameEvent.LEVEL_FAILED, OnLevelFailed);
        Messenger.RemoveListener(GameEvent.GAME_COMPLETE, OnGameComplete);
    }

    void Start() {
        OnHealthUpdated();

        //Initialize levelEnding label and pop-up to be hidden at start
        levelEnding.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            bool isShowing = popup.gameObject.activeSelf;
            popup.gameObject.SetActive(!isShowing);
            popup.Refresh();
        }
    }

    public void SaveGame() {
        Managers.Data.SaveGameState();
    }

    public void LoadGame() {
        Managers.Data.LoadGameState();
    }

    //Event listener calls function to update health label
    private void OnHealthUpdated() {
        string message = "Health: " + Managers.Player.health + "/" + Managers.Player.maxHealth;
        healthLabel.text = message;
    }

    private void OnLevelCompleted() {
        StartCoroutine(CompleteLevel());
    }

    private IEnumerator CompleteLevel() {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "Level Complete!";

        yield return new WaitForSeconds(2);

        Managers.Mission.GoToNext();
    }

    private void OnLevelFailed() {
        StartCoroutine(FailLevel());
    }

    private IEnumerator FailLevel() {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "Level Failed";

        yield return new WaitForSeconds(2);

        Managers.Player.Respawn();
        Managers.Mission.RestartCurrent();
    }

    private void OnGameComplete() {
        levelEnding.gameObject.SetActive(true);
        levelEnding.text = "You finished the Game!";
    }
}
