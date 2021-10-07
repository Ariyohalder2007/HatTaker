using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]public int id;

    [Header("Info")] 
    public float moveSpeed;

    public float jumpForce;
    public GameObject hat;

    [HideInInspector] public float curHatTime;

    [Header("Components")]
    public Rigidbody playerRb;
    public Player photonPlayer;
    public GameObject hatObj;


    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.Instance.players[id - 1] = this;
        
        //Give the Hat to the first player
        if (id==1)
        {
            GameManager.Instance.GiveHat(id, true);
        }

        if (!photonView.IsMine)
        {
            playerRb.isKinematic = true;
        }
    }


    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (curHatTime>=GameManager.Instance.timeToWin && !GameManager.Instance.gameHasEnded)
            {
                GameManager.Instance.gameHasEnded = true;
                GameManager.Instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }
        if (photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }

            if (hatObj.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }
        }
    }

    void Move()
    {
        float z = -Input.GetAxis("Horizontal")*moveSpeed;
        float x = Input.GetAxis("Vertical")*moveSpeed;

        playerRb.velocity = new Vector3(x, playerRb.velocity.y, z);
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 0.7f))
        {
            playerRb.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
        }
    }

    public void SetHat(bool hasHat)
    {
        hatObj.SetActive(hasHat);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        //Check if hitting Other player
        if (other.gameObject.CompareTag("Player"))  
        {
            // Check if the collided player has Hat?
            if (GameManager.Instance.GetPlayer(other.gameObject).id==GameManager.Instance.playerWithHat)
            {
                if (GameManager.Instance.CanGetHat())
                {
                    GameManager.Instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   
        {
            stream.SendNext(curHatTime);
        }

        if (stream.IsReading)
        {
            curHatTime = (float) stream.ReceiveNext();
        }
    }
}
