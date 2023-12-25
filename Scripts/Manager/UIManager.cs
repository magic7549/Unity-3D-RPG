using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public FadeInOut fadeInOut;
    public RestUI restUI;
    public SavepointUI savepointUI;
    public PlayerBar playerBar;
    public TalkUI talkUI;
    public InventoryUI inventoryUI;
    public ShopUI shopUI;
    public ToolTip toolTipUI;
    public LevelUpUI levelUpUI;
    public ExitUI exitUI;
    public EquipmentUI equipmentUI;
    public QuestUI questUI;
    public Ending endingUI;

    // 마우스 포인터가 UI 위에 있는지
    public bool OverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}