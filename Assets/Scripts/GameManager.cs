using System;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    //Handels All the Stuff of the Main Scene
    [Header("Stats")] public bool gameHasEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickUpTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    public int playersInGame;


    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame==PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation,
            spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        //TODO: Watch the tut at 3:40
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject==playerObj);
    }
    //Give the Hat to Player Hitting
    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        if (!initialGive)
        {
            GetPlayer(playerWithHat).SetHat(false);
        }

        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickUpTime = Time.time;
    }
    //Check if Can get the hat if the Time exceeded (just like a knock back effect)
    public bool CanGetHat()
    {
        if (Time.time > hatPickUpTime + invincibleDuration)
        
            return true;
        
        else
            return false;
    }

    [PunRPC]
    public void WinGame(int playerId)
    {
        gameHasEnded = true;
        PlayerController player = GetPlayer(playerId);
        
        //Show the UI Who Has Won
        GameUI.Instance.SetWinText(player.photonPlayer.NickName);
         
        
        Invoke("GoBackToMenu", 3f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.ChangeScene("Menu");
        Destroy(NetworkManager.Instance.gameObject);

    }
    
    
}
