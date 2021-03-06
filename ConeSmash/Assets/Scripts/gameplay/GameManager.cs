﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Level level;
    private int index;

    //Control Elements
    public GameObject player;
	public GameObject controller;
    public GameObject pauseButton;

    //UI Elements
    private Text countText, timerText, finalText, highscoreText;
    private Image star1, star2, star3;
    private Text s1Txt, s2Txt, s3Txt;
    private GameObject gameoverPanel;
    private Button playagainButton, mainmenuButton;

    //Game Logic Elements
    public bool countDown;
    public bool gameOver;
    public bool paused;
	private bool newHighscore;
    private float timer;
    private int coneCount;
    private int coneTotal;

    //Game Audio
    private bool music;
    public bool sfx;

    public AudioSource audioBG;
    public AudioClip soundBG;

    private static GameManager manager;

    public static GameManager Manager
    {
        get { return manager; }
    }

    void Awake()
    {
        GetThisGameManager();
        controllerCheck();
        soundCheck();
        spawnPlayer();
    }

    void GetThisGameManager()
    {
        if (manager != null && manager != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            manager = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

        nullCheck();
        Reset();
        print("START again");
        print("Level name: " + level.getName());
        print("Level index: " + index);
        print("Highscore: " + level.getHighScore());
        print("Stars: " + level.getStars());
    }


    // Update is called once per frame
    void Update()
    {
        nullCheck();

        if (music)
        {
            if (!audioBG.isPlaying) { audioBG.PlayOneShot(soundBG); }
        } else if (!music)
        {
            audioBG.Stop();
        }

        //Counts down from defined "timer" to reach Game Over.
        if (!gameOver && !paused && countDown)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F2");
            if (timer <= 0)
            {
                gameFinished();
                print("GAME OVER");
            }
        }
        
    }

    //Checks for chosen control scheme
    void controllerCheck()
    {
        if(!PlayerPrefs.HasKey("Control") || PlayerPrefs.GetString("Control") == "classic")
        {
            controller =  (GameObject) Instantiate(Resources.Load("Prefabs/ControlClassic"));

        } else
        {
            controller = (GameObject)Instantiate(Resources.Load("Prefabs/ControlTilt"));
        }

    }

    void soundCheck()
    {
        string m = PlayerPrefs.GetString("Music");
        string s = PlayerPrefs.GetString("Sfx");

        if (m == null || m == "on"){ music = true; } else { music = false; }
        if (s == null || s == "on") { sfx = true; } else { sfx = false; }

    }

    void spawnPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            int choice;
            Vector3 playerSpawn = GameObject.Find("playerSpawn").transform.position;

            if (PlayerPrefs.HasKey("selectedBall"))
            {
                choice = PlayerPrefs.GetInt("selectedBall");
            }
            else {
                choice = 1;
            }

            switch (choice)
            {
                case 1:
                    player = (GameObject)Instantiate(Resources.Load("Prefabs/player_football"));
                    player.transform.position = playerSpawn;
                    break;
                case 2:
                    player = (GameObject)Instantiate(Resources.Load("Prefabs/player_basketball"));
                    player.transform.position = playerSpawn;
                    break;
                case 3:
                    player = (GameObject)Instantiate(Resources.Load("Prefabs/player_bowling"));
                    player.transform.position = playerSpawn;
                    break;
                default:
                    print("Error case at playerSpawn");
                    break;
            }
        }
    }

    public void soundOn(string x)
    {
        if(x == "music") { music = true; }
        if (x == "sfx") { sfx = true; }
    }

    public void soundOff(string x)
    {
        if (x == "music") { music = false; }
        if (x == "sfx") { sfx = false; }
    }

    //SCORING METHODS
    //Initially counting total cones on startup
    int countCones()
    {
        int count = GameObject.FindGameObjectsWithTag("Cone").Length;
        print("TOTAL CONES: " + count.ToString());
        return count;
    }

    //Incrementing/decrementing score through calls from coneScript
    public void AddScore(int x, GameObject cone)
    {
        coneCount = coneCount + x;
        SetCountText();
    }

    //Updating score text
    public void SetCountText()
    {
        countText.text = coneCount.ToString() + " / " + coneTotal.ToString();
    }

    //GAME OVER METHODS
    //Called upon gameover, disable Player/HUD and display game over panel with final score/play again button/main menu button
    void gameFinished()
    {
        gameOver = true;
        paused = true;
        countText.enabled = false;
        timerText.enabled = false;
        GameObject.FindGameObjectWithTag("Player").SetActive(false);
        gameoverPanel.SetActive(true);

        checkStars ();
        checkHighScore();

        playagainButton.onClick.AddListener(delegate { Reset(); });
        mainmenuButton.onClick.AddListener(delegate { MainMenu(); });

    }

    //TODO It works...but please make this much much nicer. Still not great.
	void checkStars(){
        if(coneCount > (coneTotal * 0.6f))
        {
            s3Txt.text = "";
            s2Txt.text = "";
            s1Txt.text = "";
            star3.color = Color.yellow;
            star2.color = Color.yellow;
            star1.color = Color.yellow;
            if(level.getStars() < 3) { level.setStars(3); }
        } else if (coneCount > (coneTotal * 0.4f))
        {
            s3Txt.text = Mathf.Round(coneTotal * 0.6f).ToString();
            s2Txt.text = "";
            s1Txt.text = "";
            star2.color = Color.yellow;
            star1.color = Color.yellow;
            if (level.getStars() < 2) { level.setStars(2); }
        } else if (coneCount > (coneTotal * 0.2f))
        {
            s3Txt.text = Mathf.Round(coneTotal * 0.6f).ToString();
            s2Txt.text = Mathf.Round(coneTotal * 0.4f).ToString();
            s1Txt.text = "";
            star1.color = Color.yellow;
            if (level.getStars() < 1) { level.setStars(1); }
        } else {
            s3Txt.text = Mathf.Round(coneTotal * 0.6f).ToString();
            s2Txt.text = Mathf.Round(coneTotal * 0.4f).ToString();
            s1Txt.text = Mathf.Round(coneTotal * 0.2f).ToString();
        }

    }

    void checkHighScore()
    {
		if (level.getHighScore() < coneCount)
        {
			level.setHighScore(coneCount);
            newHighscore = true;
        }
        if (newHighscore)
        {
            highscoreText.text = "NEW HIGHSCORE!";
        }
        else {
			highscoreText.text = "High Score: " + level.getHighScore();
        }
        finalText.text = "You smashed " + coneCount + " out of " + coneTotal + " cones in " + level.getStartTime() + " seconds!";
    }
    
    //Public bool to check for gameover condition
    public bool isGameOver()
    {
        return gameOver;
    }

    //Resets the game loop upon pressing play again
    public void Reset()
    {
        countDown = false;
        gameOver = false;
        paused = false;
        newHighscore = false;
        index = level.getIndex();
        timer = level.getStartTime();
        coneCount = 0;
        coneTotal = countCones();
        gameoverPanel.SetActive(false);

        audioBG = gameObject.GetComponent<AudioSource>();
        soundBG = (AudioClip)Instantiate(Resources.Load(level.getMusic()));
      

        star1.color = Color.grey;
        star2.color = Color.grey;
        star3.color = Color.grey;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Returns to main menu upon pressing main menu
    public void MainMenu()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
    }

    //Checks initilization of UI elements
    void nullCheck()
    {
        if (level == null) { level = GameObject.Find("levelID").GetComponent<Level>(); }
        if(controller == null) { controller = GameObject.FindGameObjectWithTag("GameController"); }
        if (countText == null){ countText = GameObject.Find("GameUI/Count Text").GetComponent<Text>();}
        if(timerText == null){ timerText = GameObject.Find("GameUI/Timer Text").GetComponent<Text>();}
        if (finalText == null){ finalText = GameObject.Find("GameUI/GameOver Panel/final Text").GetComponent<Text>();}
        if (highscoreText == null) { highscoreText = GameObject.Find("GameUI/GameOver Panel/highscore Text").GetComponent<Text>(); }
        if (star1 == null){ star1 = GameObject.Find("GameUI/GameOver Panel/star1").GetComponent<Image>();}
		if (star2 == null){ star2 = GameObject.Find("GameUI/GameOver Panel/star2").GetComponent<Image>();}
		if (star3 == null){ star3 = GameObject.Find("GameUI/GameOver Panel/star3").GetComponent<Image>();}
        if (s1Txt == null) { s1Txt = GameObject.Find("GameUI/GameOver Panel/star1/Text").GetComponent<Text>(); }
        if (s2Txt == null) { s2Txt = GameObject.Find("GameUI/GameOver Panel/star2/Text").GetComponent<Text>(); }
        if (s3Txt == null) { s3Txt = GameObject.Find("GameUI/GameOver Panel/star3/Text").GetComponent<Text>(); }
        if (gameoverPanel == null){ gameoverPanel = GameObject.Find("GameUI/GameOver Panel");}
        if (playagainButton == null){ playagainButton = GameObject.Find("GameUI/GameOver Panel/PlayAgain Button").GetComponent<Button>();}
        if (mainmenuButton == null) { mainmenuButton = GameObject.Find("GameUI/GameOver Panel/MainMenu Button").GetComponent<Button>(); }
        if (pauseButton == null) { pauseButton = GameObject.Find("GameUI/Pause Button"); }

        if (!gameOver && gameoverPanel.activeSelf){ gameoverPanel.SetActive(false);}
    }

}
