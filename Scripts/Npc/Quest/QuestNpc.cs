using System.Collections;
using System.Collections.Generic;

public class QuestNpc
{
    public string questName;
    public int[] npcId;

    public QuestNpc(string name, int[] id) //생성자
    {
        questName = name;
        npcId = id;
    }
}


