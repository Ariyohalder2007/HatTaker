using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // [Header("Components")]
    // public PhotonView photonView;

    // instance
    [HideInInspector]public static NetworkManager Instance;

    void Awake ()
    {

        if (Instance!=null)
        {
            
            Destroy(gameObject);
            return;
            
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start ()
    {
        
        
        
        
        
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    // attempts to CREATE a new room
    public void CreateRoom (string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    // attempts to JOIN a room
    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // changes the scene using Photon's system
    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}