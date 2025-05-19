using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    public static NetworkRunner runnerInstance;
    [SerializeField] private string lobbyName = "default";


    [SerializeField] private Transform sessionListContentParet;
    [SerializeField] private GameObject sessionListEntryPrefab;
    [SerializeField] private Dictionary<string, GameObject> sessionListUiDictionary = new Dictionary<string, GameObject>();

    [SerializeField] private SceneAsset gameplayScene;
    [SerializeField] private SceneAsset lobbyScene;
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] private GameObject customRoomCanvas;
    [SerializeField] private TMP_InputField sessionNameInputField;
    [SerializeField] private GameObject mainMenuCanvas;

    private void Awake()
    {
        runnerInstance = gameObject.GetComponent<NetworkRunner>();

        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }


    private void Start()
    {
        // Conexi�n con el servidor con el modo de juego establecido
        //          SessionLobby.Shared
        //          SessionLobby.ClientServer
        //
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }


    public static void ReturnToLobby()
    {
        NetworkManager.runnerInstance.Despawn(runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer)); 
        NetworkManager.runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(lobbyScene.name);
    }


    //M�todo para crear salas aleatorias    
    public void CreateRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Room-" + randomInt.ToString();
        
        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(gameplayScene.name)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
            PlayerCount = 4,
            IsVisible = true,
        });
    }
    
    

    private int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if(name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Eliminamos el listado
        DeleteOldSessionFromUI(sessionList);

        // Volvemos a generarlo
        CompareLists(sessionList);

        
                
    }


    private void CompareLists(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (sessionListUiDictionary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }   
        }    
    }

    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParet;
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        sessionListUiDictionary.Add(session.Name, newEntry);

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;


        newEntry.SetActive(session.IsVisible);
    }


    private void UpdateEntryUI(SessionInfo session)
    {
        
        sessionListUiDictionary.TryGetValue(session.Name, out GameObject newEntry);
       
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        
        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;


        newEntry.SetActive(session.IsVisible);
    }


    private void DeleteOldSessionFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;

        foreach(KeyValuePair <string, GameObject> kvp in sessionListUiDictionary)
        {
            string sessionKey = kvp.Key;

            foreach (SessionInfo sessionInfo in sessionList)
            {
                if(sessionInfo.Name == sessionKey)
                {
                    isContained = true;
                    break;
                }
            }

            if (!isContained)
            {
                uiToDelete = kvp.Value;
                sessionListUiDictionary.Remove(sessionKey);
                Destroy(uiToDelete);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer) 
        {
            
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
            runner.SetPlayerObject(player, playerObject);
        }
        
    }
    
    public void ShowCustomRoomUI()
    {
        customRoomCanvas.SetActive(true);
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
    }
    
    public void CreateCustomNamedSession()
    {
        string customName = sessionNameInputField.text;
        string sessionName;

        if (string.IsNullOrWhiteSpace(customName))
        {
            int randomInt = UnityEngine.Random.Range(1000, 9999);
            sessionName = "Room-" + randomInt.ToString();
        }
        else
        {
            sessionName = customName;
        }

        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(gameplayScene.name)),
            SessionName = sessionName,
            GameMode = GameMode.Shared,
            PlayerCount = 4,
            IsVisible = true,
        });

        customRoomCanvas.SetActive(false);
    }
    

    public void OnConnectedToServer(NetworkRunner runner)
    {
          
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

   

    

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
