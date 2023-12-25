using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] public GameObject panel;

    public GameObject weaponSlot;
    public GameObject necklaceSlot;
    public GameObject ringSlot;
    public GameObject shoesSlot;

    private bool isActive = false;

    public void OpenAndCloseEquipment()
    {
        isActive = !isActive;
        panel.SetActive(isActive);
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;

        // 장비창 열리면 카메라 회전 멈춤
        IngameManager.instance.cameraMovement.RotationControl(!isActive);
    }
}
