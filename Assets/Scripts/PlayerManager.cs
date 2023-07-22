using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    public int health {get; private set;}
    public int maxHealth {get; private set;}
    private NetworkService _network;

    public void Startup(NetworkService service) {
        Debug.Log("Player manager starting...");
        _network = service;
        UpdateDate(50,100);
        status = ManagerStatus.Started;
    }

    public void ChangeHealth(int value) {
        health += value;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0) {
            Messenger.Broadcast(GameEvent.LEVEL_FAILED);
        }
        Messenger.Broadcast(GameEvent.HEALTH_UPDATED);
    }

    public void UpdateDate(int health, int maxHealth) {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public void Respawn() {
        UpdateDate(50,100);
    }
}
