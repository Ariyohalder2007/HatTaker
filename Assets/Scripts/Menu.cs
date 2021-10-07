using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")] public GameObject mainScreen;

    public GameObject lobbyScreen;


    [Header("Lobby Screen")] public TextMeshProUGUI playerNamesList;

    [SerializeField] private Button startGameButton;


    [Header("Main Screen")] [SerializeField]
    private Button createRoomButton;

    [SerializeField] private Button joinRoomButton;


    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_Text roomName)
    {
        NetworkManager.Instance.CreateRoom(roomName.text);
    }

    public void OnJoinRoomButton(TMP_Text roomName)
    {
        NetworkManager.Instance.JoinRoom(roomName.text);
    }

    public void OnPlayerNameUpdate(TMP_Text playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerNamesList.text = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerNamesList.text += player.NickName + "\n";
            Debug.Log(player.NickName);
        }

        startGameButton.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public void LeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnGameStartButton()
    {
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, "Game");
    }
}