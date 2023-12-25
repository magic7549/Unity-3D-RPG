using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;

// 리스트를 직렬화 하기 위한 클래스
[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

// 직렬화가 안되는 Item 대신 사용
[System.Serializable]
public class InvenSave
{
    public int slotNum;
    public int itemCode;
    public int itemCount;
}

public class DB_Manager : MonoBehaviour
{
    // ExecuteReader - 데이터를 받아오는 쿼리문에 사용
    // ExecuteNonQuery - 데이터 삽입/삭제 시 사용 (반환 값 : 영향을 받은 행의 수)
    // ExecuteScalar - 하나의 값이 리턴되는 쿼리문에 사용
    // https://kugancity.tistory.com/entry/c%EC%97%90%EC%84%9C-mysql-MySqlCommand-%EC%82%AC%EC%9A%A9%ED%95%98%EA%B8%B0-%EC%98%88%EC%8B%9C

    // DataReader는 한줄씩 읽는 것이고, DataAdapter는 DataSet에 저장하는 방식 

    private static string ipAddress = "ip주소";
    private static string db_id = "db_id";
    private static string db_pw = "db_pw";
    private static string db_name = "db_name";
    private string db_Info = "Server=" + ipAddress + ";Database=" + db_name + ";Uid=" + db_id + ";Pwd=" + db_pw + ";";

    public DataTable userTable;
    public DataTable monsterTable;
    public DataTable levelTable;
    public DataTable expTable;

    public int[] slot = new int[3];
    public string user_id;

    private void Awake()
    {
        userTable = new DataTable();
        monsterTable = new DataTable();
        levelTable = new DataTable();
        expTable = new DataTable();
    }

    public bool OnLogin(string id, string pw)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "select * from user where user_id=\"" + id + "\";";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                // MySqlDataReader는 하나의 Connection에 하나만 Open되어 있어야 함

                while (reader.Read())
                {
                    if (reader[1].ToString().Equals(pw))
                    {
                        // 유저 id 저장
                        user_id = reader[0].ToString();

                        // 캐릭터 존재 여부
                        slot[0] = reader["slot1"].GetHashCode();
                        slot[1] = reader["slot2"].GetHashCode();
                        slot[2] = reader["slot3"].GetHashCode();

                        LoadCharacterDB();
                        LoadMonsterDB();
                        LoadExpDB();
                        LoadLevelDB();

                        return true;
                    }
                    else
                    {
                        Debug.Log("비밀번호 일치하지 않음");
                        return false;
                    }
                }

                Debug.Log("아이디 없음");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
        return false;
    }

    public int OnRegister(string id, string pw)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "insert into user(user_id, user_password) values (\"" + id + "\", \"" + pw + "\");";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    Debug.Log("회원가입 성공");
                    return 200;
                }
            }
        }
        catch (MySqlException e)
        {
            if (e.Number == 1062) // MySQL error code for duplicate entry
            {
                Debug.Log("중복된 아이디입니다.");
                return 400;
            }
            else
            {
                Debug.Log("에러 발생: " + e.ToString());
                return 401;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
            return 402;
        }
        return 403;
    }

    public void OnCreateCharacter(int slot_num)
    {
        // 캐릭터 생성 시 기본 스탯 변경은 mysql default값 변경하기로...
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();
                Debug.Log("연결 성공");

                string query = "insert into character_table(user_id, slot, inventory) values (\"" + user_id + "\", \"slot" + slot_num + "\", \'{\"target\":[]}\');";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                if (cmd.ExecuteNonQuery() != -1)
                {
                    Debug.Log("캐릭터 생성");
                }

                query = "update user set slot" + slot_num + "=1 where user_id=\"" + user_id + "\"";
                cmd = new MySqlCommand(query, connection);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    Debug.Log("유저 정보 변경 완료");

                    slot[slot_num - 1] = 1;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void LoadCharacterDB()
    {
        userTable = new DataTable();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "select * from character_table where user_id=\"" + user_id + "\"";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                adapter.Fill(userTable);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void LoadMonsterDB()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "select * from monster";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                adapter.Fill(monsterTable);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void LoadLevelDB()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "select * from level_table";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                adapter.Fill(levelTable);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void LoadExpDB()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "select * from exp_table";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                adapter.Fill(expTable);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SaveInventory()
    {
        // Item 자체는 직렬화가 불가능하여 Json으로 저장x
        // 직렬화 가능한 InvenSave를 생성하여 직렬화
        List<InvenSave> inven = new List<InvenSave>();
        foreach (KeyValuePair<int, Items> item in IngameManager.instance.inventory.invenData)
        {
            InvenSave temp = new InvenSave();
            temp.slotNum = item.Key;
            temp.itemCode = item.Value.item.itemCode;
            temp.itemCount = item.Value.itemCount;
            inven.Add(temp);
        }

        string json = JsonUtility.ToJson(new Serialization<InvenSave>(inven)); // 제이슨화
        Debug.Log(json);

        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                string query = "UPDATE character_table SET inventory = \'" + json + "\' WHERE user_id = \"" + user_id + "\" AND slot = \"slot" + SystemManager.instance.selectSlotNum + "\";";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    Debug.Log("유저 정보 변경 완료");
                }
                connection.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public InvenData LoadInventory()
    {
        string slot_str = "slot" + SystemManager.instance.selectSlotNum;
        for (int i = 0; i < userTable.Rows.Count; i++)
        {
            if (userTable.Rows[i]["slot"].ToString().Equals(slot_str))
            {
                // 직렬화가 안되는 Item 대신 InvenSave를 이용하여 Load
                string json = (string)userTable.Rows[i]["inventory"];
                List<InvenSave> inven = JsonUtility.FromJson<Serialization<InvenSave>>(json).ToList();

                InvenData items = new InvenData();
                for (int j = 0; j < inven.Count; j++)
                {
                    Items temp = new Items();
                    temp.item = IngameManager.instance.itemDatabase.itemDB[inven[j].itemCode];
                    temp.itemCount = inven[j].itemCount;

                    items.Add(inven[j].slotNum, temp);
                }

                return items;
            }
        }
        return null;
    }

    public int[] LoadEquipment()
    {
        string slot_str = "slot" + SystemManager.instance.selectSlotNum;
        for (int i = 0; i < userTable.Rows.Count; i++)
        {
            if (userTable.Rows[i]["slot"].ToString().Equals(slot_str))
            {
                string temp = (string)userTable.Rows[i]["equipment"];
                temp = temp.Substring(1, temp.Length - 2);
                string[] tempArray = temp.Split(',');
                int[] equipment = new int[tempArray.Length];
                for (int j = 0; j < tempArray.Length; j++)
                {
                    equipment[j] = int.Parse(tempArray[j]);
                }

                return equipment;
            }
        }
        return null;
    }

    public void SaveStats()
    {
        string slot_str = "slot" + SystemManager.instance.selectSlotNum;
        PlayerStat playerStat = IngameManager.instance.stat;

        string skill_unlock_str = "[" + string.Join(", ", playerStat.skill_unlock) + "]";
        string savepoint_unlock_str = "[" + string.Join(", ", playerStat.savepoint_unlock) + "]";
        string quest_str = "[" + string.Join(", ", playerStat.quest) + "]";
        string equipment_str = "[" + string.Join(", ", IngameManager.instance.equipment.equipment) + "]";

        string query = $@"
            UPDATE character_table
            SET
                level = {playerStat.level},
                exp = {playerStat.exp},
                money = {playerStat.money},
                maxHp = {playerStat.maxHp},
                currHp = {playerStat.currHp},
                maxMp = {playerStat.maxMp},
                currMp = {playerStat.currMp},
                damage = {playerStat.damage},
                speed = {playerStat.speed},
                quest = '{quest_str}',
                skill_unlock = '{skill_unlock_str}',
                savepoint_unlock = '{savepoint_unlock_str}',
                lastSavepoint = {playerStat.lastUseSavepoint},
                equipment = '{equipment_str}'
            WHERE user_id = @user_id AND slot = @slot;
        ";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(db_Info))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@slot", slot_str);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}