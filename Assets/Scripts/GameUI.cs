
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;
    public static GameUI Instance;
    
    

    private void Start()
    {
        Instance = this;
        InitializePlayerUI();
    }

    private void Update()
    {
        UpdatePlayerUI();
    }

    void InitializePlayerUI()
    {
        for (int i = 0; i < playerContainers.Length; i++)
        {
            PlayerUIContainer container = playerContainers[i];

            if (i<PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.playerNameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.hatTimeSlider.maxValue = GameManager.Instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    void UpdatePlayerUI()
    {
        for (int x = 0; x < GameManager.Instance.players.Length; x++)
        {
            if (GameManager.Instance.players[x] != null)
            {
                playerContainers[x].hatTimeSlider.value = GameManager.Instance.players[x].curHatTime;
            }
        }
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName+" Wins!";
    }
    
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI playerNameText;
    public Slider hatTimeSlider;
}
