using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public static MainManager Instance;
    public Text TextNameValue;
    public Text TextScoreValue;
    public Text TextHSNameValue;
    public Text TextHSScoreValue;
    public bool m_PlayClicked = false;
    public bool m_ResetClicked = false;

    public GameObject GameOverText;
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;


    void Start()
    {
        Instance = this;
        TextNameValue.text = MenuManager.Instance.PlayerName;
        TextHSNameValue.text = MenuManager.Instance.HighScoreName;
        TextHSScoreValue.text = MenuManager.Instance.HighScoreValue;

        InitGame();
    }

    private void Update()
    {

        if (!m_Started)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            if (m_PlayClicked) 
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                m_PlayClicked=false;
            }
        }
        else if (m_GameOver)
        {
            if (m_ResetClicked)
            {
                m_ResetClicked = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        TextScoreValue.text = m_Points.ToString();
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        if (int.Parse(TextScoreValue.text) > int.Parse(TextHSScoreValue.text))
        {
            TextHSScoreValue.text = TextScoreValue.text;
            TextHSNameValue.text = TextNameValue.text;
        }
        MenuManager.Instance.PlayerName = TextNameValue.text;
        MenuManager.Instance.PlayerScore = TextScoreValue.text;
        MenuManager.Instance.HighScoreName = TextHSNameValue.text;
        MenuManager.Instance.HighScoreValue = TextHSScoreValue.text;
        MenuManager.Instance.UpdateLastGameResult();
    }

    public void InitGame()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    public void Play()
    {
        if (!m_Started && !m_GameOver) { m_PlayClicked = true; }
    }
    public void ResetBtn() { m_ResetClicked = true; }
    public void Menu() 
    { 
        MenuManager.Instance.SaveScoreList(); 
        SceneManager.LoadScene(0); 
    }
    public void Exit() 
    { 
        MenuManager.Instance.SaveScoreList();
        MenuManager.Instance.ExitGame();
    }
}
