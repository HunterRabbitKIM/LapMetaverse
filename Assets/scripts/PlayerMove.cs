using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public float moveSpeed;

    public float sensitivity;

    public float cameraRotationLimit;
    public float currentCameraRotationX;

    public Camera mainCamera;
    public Rigidbody rigidbody;

    public Text NickNameText;
    public PhotonView PV;
    public GameObject GO;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    private void Update()
    {
        Move();
        CameraRotation();
        CharacterRoatation();

    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * moveSpeed;

        //rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);

        PV.RPC("MoveRPC", RpcTarget.AllBuffered, velocity);

        if (PV.IsMine)
        {
            
        }
    }

    [PunRPC]
    private void MoveRPC(Vector3 velocity)
    {
        if(!PV.IsMine)
        {
            rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * sensitivity;

        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        //mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        PV.RPC("CameraRotationRPC", RpcTarget.All, currentCameraRotationX);

        if (PV.IsMine)
        {
            
        }
        
    }

    [PunRPC]
    private void CameraRotationRPC(float rotationX)
    {
        if(!PV.IsMine)
        {
            mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    private void CharacterRoatation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRoationY = new Vector3(0f, yRotation, 0f) * sensitivity;
        //rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(characterRoationY));

        PV.RPC("CharacterRotationRPC", RpcTarget.AllBuffered, characterRoationY);

        if (PV.IsMine)
        {
            
        }
        
    }

    [PunRPC]
    private void CharacterRotationRPC(Vector3 rotationY)
    {
        if(!PV.IsMine)
        {
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotationY));
        }
    }
    
}
