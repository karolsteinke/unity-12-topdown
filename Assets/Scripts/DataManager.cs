using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}
    private string _filename;
    private NetworkService _network;

    public void Startup(NetworkService service) {
        Debug.Log("Data manager starting...");
        _network = service;
        //Construct full path to the game.dat file
        //persistentDataPath is a location which Unity provides to store data in
        _filename = Path.Combine(Application.persistentDataPath, "game.dat");
        status = ManagerStatus.Started;
    }

    public void SaveGameState() {
        //Dictionary that will be serialized
        Dictionary<string, object> gamestate = new Dictionary<string, object>();
        gamestate.Add("inventory", Managers.Inventory.GetData());
        gamestate.Add("health", Managers.Player.health);
        gamestate.Add("maxHealth", Managers.Player.maxHealth);
        gamestate.Add("curLevel", Managers.Mission.curLevel);
        gamestate.Add("maxLevel", Managers.Mission.maxLevel);

        //Create a file at the file path
        FileStream stream = File.Create(_filename);
        BinaryFormatter foramtter = new BinaryFormatter();
        //Serialize Dicttionary 'gamestate' as contents of the created file
        foramtter.Serialize(stream, gamestate);
        stream.Close();
    }

    public void LoadGameState() {
        //Only continue to load if the file exists
        if (!File.Exists(_filename)) {
            Debug.Log("No saved game");
            return;
        }

        //Dictionary to put loaded data in
        Dictionary<string, object> gamestate;

        BinaryFormatter foramtter = new BinaryFormatter();
        FileStream stream = File.Open(_filename, FileMode.Open);
        gamestate = foramtter.Deserialize(stream) as Dictionary<string, object>;
        stream.Close();

        //Update managers with deserialized data
        Managers.Inventory.UpdateData((Dictionary<string, int>) gamestate["inventory"]);
        Managers.Player.UpdateDate((int) gamestate["health"], (int) gamestate["maxHealth"]);
        Managers.Mission.UpdateData((int) gamestate["curLevel"], (int) gamestate["maxLevel"]);
        Managers.Mission.RestartCurrent();
    }
}