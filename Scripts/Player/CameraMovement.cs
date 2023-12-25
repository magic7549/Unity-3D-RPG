using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectToFollow;
    public Transform realCamera;
    public float followSpeed = 10f;
    public float sensitivity = 500f;
    public float clampAngle = 70f;  // 상하 각도 제한
    public float smoothness = 10f;
    public float minDistance;
    public float maxDistance;
    public float distanceFix;
 
    private Vector3 dirNormalized;
    private Vector3 finalDir;
    private float finalDistance;
    private float rotX;
    private float rotY;
    private bool canControlRotation = true;
    private bool canControlMove = true;

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
    }

    private void Update()
    {
        if (canControlRotation)
        {
            // 카메라 컨트롤 로직
            rotX -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
            transform.rotation = rot;
        }
    }

    private void LateUpdate()
    {
        if (canControlMove && objectToFollow != null)
        {
            // 카메라 컨트롤 로직
            //transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);
            transform.position = objectToFollow.position;

            finalDir = transform.TransformPoint(dirNormalized * maxDistance);

            // 카메라와 캐릭터 사이에 오브젝트가 있을경우 카메라 거리 조절
            RaycastHit hit;
            // Everything에서 Player, Monster, MonsterDead, SafeZone, SavePoint, Ignore Raycast 레이어만 제외하고 충돌 체크함
            int layerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Monster")) | (1 << LayerMask.NameToLayer("MonsterDead")) | (1 << LayerMask.NameToLayer("SafeZone")) | (1 << LayerMask.NameToLayer("SavePoint")) | (1 << LayerMask.NameToLayer("Ignore Raycast")));
            layerMask = ~layerMask;
            if (Physics.Linecast(transform.position, finalDir, out hit, layerMask))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                finalDistance = maxDistance;
            }
            realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
            Debug.DrawLine(transform.position, finalDir * finalDistance, Color.green);
        }
    }

    // 카메라 움직임 제어
    public void MoveControl(bool isControl)
    {
        canControlMove = isControl;
    }

    // 카메라 회전 제어
    public void RotationControl(bool isControl)
    {
        canControlRotation = isControl;
    }
}
