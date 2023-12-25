using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    [SerializeField]
    public GameObject exitPanel;
    private bool exit = false;

    private void Start()
    {
        exitPanel.SetActive(false);
    }

    public void OpenAndCloseExit()
    {
        exit = !exit;
        exitPanel.SetActive(exit);
        Cursor.visible = exit;
        Cursor.lockState = exit ? CursorLockMode.None : CursorLockMode.Locked;

        // 인벤토리 열리면 카메라 회전 멈춤
        IngameManager.instance.cameraMovement.RotationControl(!exit);
    }

    public void ExitButton()
    {
        IngameManager.instance.playerController.gameObject.GetComponent<Exit>().LeaveRoom();
    }
}