using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerManager))]
[RequireComponent (typeof(InventoryManager))]
[RequireComponent (typeof(MissionManager))]

public class Managers : MonoBehaviour {
    public static PlayerManager Player {get; private set;} //why capital case?
    public static InventoryManager Inventory {get; private set;}
    public static MissionManager Mission {get; private set;}
    public static DataManager Data {get; private set;}
    private List<IGameManager> _startSequence;

    void Awake() {
        //Unity's command to persist an object between scenes
        DontDestroyOnLoad(gameObject);

        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Mission = GetComponent<MissionManager>();
        Data = GetComponent<DataManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(Mission);
        _startSequence.Add(Data);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() {
        NetworkService network = new NetworkService();
        
        //Call startup for all modules
        foreach (IGameManager manager in _startSequence) {
            manager.Startup(network);
        }

        //pause for one frame
        yield return null;

        //do while loop unitl all modules ready
        int numReady = 0;
        int numModules = _startSequence.Count;
        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager manager in _startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }
            if (numReady > lastReady) {
                Debug.Log("Managers progress: " + numReady + "/" + numModules);
                Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }
            //pause for one frame
            yield return null;
        }
        
        Debug.Log("All managers started up");
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }
}
