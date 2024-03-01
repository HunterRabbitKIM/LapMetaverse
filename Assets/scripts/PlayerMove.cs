using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public float moveSpeed; // 움직임
    public float sensitivity; // 감도(마우스)
    public float cameraRotationLimit; // 카메라 회전
    public float currentCameraRotationX;

    public Camera mainCamera; // 메인 카메라
    public Rigidbody rigidbody; // 움직임

    public Text NickNameText; // 닉네임

    public PhotonView PV;
    public GameObject GO;

    Vector3 curPos;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        if(PV.IsMine == true)
        {
            mainCamera.gameObject.SetActive(true);

            /*
            // PhotonTransformView 컴포넌트를 추가합니다.
            PhotonTransformView photonTransformView = gameObject.AddComponent<PhotonTransformView>();
            photonTransformView.m_SynchronizePosition = true;
            photonTransformView.m_SynchronizeRotation = true;
            */
        }
        /*
        else
        {
            Destroy(mainCamera);
            Destroy(GetComponent<PlayerMove>());
        }
        */
    }

    

    private void Update()
    {
        if(PV.IsMine)
        {
            Move();
            CameraRotation();
            CharacterRoatation();
        }
    }

    private void Move()
    {
        if (PV.IsMine)
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");
            Vector3 moveHorizontal = transform.right * moveDirX;
            Vector3 moveVertical = transform.forward * moveDirZ;

            Vector3 velocity = (moveHorizontal + moveVertical).normalized * moveSpeed;

            //rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);

            PV.RPC("MoveRPC", RpcTarget.AllBuffered, velocity);
        }
    }

    
    [PunRPC]
    private void MoveRPC(Vector3 velocity)
    {
        rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
    }
    

    private void CameraRotation()
    {
        if (PV.IsMine)
        {
            float xRotation = Input.GetAxisRaw("Mouse Y");
            float cameraRotationX = xRotation * sensitivity;

            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

            PV.RPC("CameraRotationRPC", RpcTarget.AllBuffered, currentCameraRotationX);
        }
        
    }

    
    [PunRPC]
    private void CameraRotationRPC(float rotationX)
    {
        mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    

    private void CharacterRoatation()
    {
        if (PV.IsMine)
        {
            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 characterRoationY = new Vector3(0f, yRotation, 0f) * sensitivity;
            
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(characterRoationY));

            PV.RPC("CharacterRotationRPC", RpcTarget.AllBuffered, characterRoationY);
        }
        
    }

    
    [PunRPC]
    private void CharacterRotationRPC(Vector3 rotationY)
    {
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotationY));
    }
    

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
    

}
