using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;
using TMPro;
using System.IO;
using static MenuManager;
using System.Xml.Linq;
//using System;
//using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be
// initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public GameObject TMP_InputField_Name;
    public TextMeshProUGUI TMP_MessageField;
    public List<PlayerInfo> PlayerScores;
    public string PlayerName;
    public string PlayerScore;
    public string HighScoreName;
    public string HighScoreValue;

    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
    //[SerializeField] private int m_ItemsToGenerate;
    
    private void Start()
    {
        Instance = this;
        LoadScoreList();
        PopulateScrollView();
    }
    
    public void PopulateScrollView()
    {
        foreach (PlayerInfo playerinfo in PlayerScores)
        {
            var item_go = Instantiate(m_ItemPrefab);
            item_go.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = playerinfo.Name;
            item_go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerinfo.Score;
            item_go.transform.SetParent(m_ContentContainer);
            item_go.transform.localScale = Vector2.one; //reset item's scale -- can get munged w/UI prefabs
        }
    }
    public void OnInputNewName()
    {
        PlayerName = TMP_InputField_Name.GetComponent<TMP_InputField>().text;
        bool playerExists = false;
        foreach (PlayerInfo playerinfo in PlayerScores) {
            if (playerinfo.Name == PlayerName)
            {
                playerExists = true;
                break;
            }
        }
        if (!playerExists)
        {
            var item_go = Instantiate(m_ItemPrefab);
            item_go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerName;
            item_go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
            item_go.transform.SetParent(m_ContentContainer);
            item_go.transform.localScale = Vector2.one; //reset item's scale -- can get munged w/UI prefabs
            var data = new PlayerInfo();
            data.Name = PlayerName;
            data.Score = "0";
            PlayerScores.Add(data);
        }
    }
    public void GetHighScore()
    {
        string HSName = "n/a";
        int HSScore = 0;
        foreach (PlayerInfo playerinfo in PlayerScores)
        {
            if ( int.Parse(playerinfo.Score) > HSScore )
            {
                HSScore = int.Parse(playerinfo.Score);
                HSName = playerinfo.Name;
            }
        }
        HighScoreName = HSName;
        HighScoreValue = HSScore.ToString();
    }
    public void UpdateLastGameResult()
    {
        foreach (PlayerInfo playerinfo in PlayerScores)
        {
            if (playerinfo.Name == PlayerName)
            {
                if (int.Parse(PlayerScore) > int.Parse(playerinfo.Score))
                {
                    playerinfo.Score = PlayerScore;
                }
            }
        }
        Debug.Log("UpdateLastGameResult");
    }

    public void StartGame()
    {
        if (PlayerName.Length > 0)
        {
            TMP_MessageField.text = "";
            GetHighScore();
            SceneManager.LoadScene(1);
        } else
        {
            TMP_MessageField.text = "ALERT: Enter a player name and press START";
        }

    }
    public void ExitGame()
    {
        SaveScoreList();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string Name;
        public string Score;
    }

    [System.Serializable]
    public class PlayerList
    {
        public List<PlayerInfo> PlayerScores;
    }

    public void SaveScoreList()
    {
        var data = new PlayerList();
        data.PlayerScores = PlayerScores;
        string json = JsonUtility.ToJson(data);
        Debug.Log(json);
        //File.WriteAllText(Application.persistentDataPath + "/scoreHistory.json", json);
        File.WriteAllText("H:/Unity/mytemp/scoreHistory.json", json);
    }
    public void LoadScoreList()
    {
        //string path = Application.persistentDataPath + "/scoreHistory.json";
        string path = "H:/Unity/mytemp/scoreHistory.json";
        if (File.Exists(path))
        {
            var data = new PlayerList();
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<PlayerList>(json);
            PlayerScores = data.PlayerScores;
        }
    }
}