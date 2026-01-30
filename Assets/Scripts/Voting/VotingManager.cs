using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using TMPro; // if using text mesh pro use

public class VotingManager : MonoBehaviour
{
    [Header("UI References")]
    public Image[] npcImages = new Image[4];
    public Text[] voteTexts = new Text[4];
    public Text timerText;
    public Sprite[] npcPortraits; // All available NPC portraits (assign 10+ in Inpsector)
    public Canvas votingCanvas;

    [Header("Config")]
    public float voteTime = 10f; // 10 Seconds for voting

    private Image[][] playerHighlights;
    private KeyCode[,] playerInputKeys = new KeyCode[4, 5] // 4 players, 5 keys each, WASD + E for player 1, Arrow keys + Enter for player 2, etc.

    {
        { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, KeyCode.E },
        { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Return },
        { KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K, KeyCode.O},
        { KeyCode.F, KeyCode.H, KeyCode.T, KeyCode.G, KeyCode.Y}
    };

    private int[] playerSelections = new int[4];
    private bool[] hasVoted = new bool[4];
    private int[] voteCounts = new int[4];
    private float voteTimer;
    private bool votingActive = false;

    void Start()
    {
        // Auto-find Highlights
        playerHighlights = new Image[4][];
        for (int i = 0; i < 4; i++)
        {
            playerHighlights[i] = new Image[4];
            for (int j = 0; j < 4; j++)
            { 
                Transform highlightTf = npcImages[j].transform.Find($"I{i + 1}Highlight");
                if (highlightTf != null) {
                    playerHighlights[i][j] = highlightTf.GetComponent<Image>();
                    playerHighlights[i][j].gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Highlight for Player {i + 1} on NPC {j + 1} not found. Please ensure the hierarchy is correct.");
                }

            }

        }
    }

    void Update()
    {
        if (!votingActive) return;

        HandleInputs();
        voteTimer -= Time.deltaTime;
        timerText.text = Mathf.Ceil(voteTimer).ToString();

        if (voteTimer <= 0f)
        {
            EndVote();
        }
    }

    private void HandleInputs()
    {
        for (int i = 0; i <4; i++) //for each players
        {
            if (hasVoted[i]) continue; //skip if already voted

            if(Input.GetKeyDown(playerInputKeys[i, 0])) // Left
            {
                int newSel = Left(playerSelections[i]);
                if (newSel != playerSelections[i]) MoveHighlight(i, newSel); // Update highlight only if selection changed
            }
            else if (Input.GetKeyDown(playerInputKeys[i, 1])) // Right
            {
                int newSel = Right(playerSelections[i]);
                if (newSel != playerSelections[i]) MoveHighlight(i, newSel); // Update highlight only if selection changed
            }
            else if (Input.GetKeyDown(playerInputKeys[i, 2])) // Up
            {
                int newSel = Up(playerSelections[i]);
                if (newSel != playerSelections[i]) MoveHighlight(i, newSel); // Update highlight only if selection changed
            }
            else if (Input.GetKeyDown(playerInputKeys[i, 3])) // Down
            {
                int newSel = Down(playerSelections[i]);
                if (newSel != playerSelections[i]) MoveHighlight(i, newSel); // Update highlight only if selection changed
            }
            else if (Input.GetKeyDown(playerInputKeys[i, 4])) // Vote
            {
                CastVote(i);
            }
        }
    }

    private int Left(int cur) => (cur % 2 == 0) ? cur : cur - 1; // Assumes 2x2 grid layout
    private int Right(int cur) => (cur % 2 == 1) ? cur : cur + 1; // Assumes 2x2 grid layout
    private int Up(int cur) => (cur >= 2) ? cur - 2 : cur; // Assumes 2x2 grid layout
    private int Down(int cur) => (cur < 2) ? cur + 2 : cur; // Assumes 2x2 grid layout

    private void MoveHighlight(int p, int newSelection)
    {
        playerHighlights[p][playerSelections[p]].gameObject.SetActive(false); // Hide old highlight
        playerHighlights[p][newSelection].gameObject.SetActive(true); //    Show new highlight
        playerSelections[p] = newSelection;// Update selection
    }

    private void CastVote(int p)
    { 
        int votedSlot = playerSelections[p];
        voteCounts[votedSlot]++;
        voteTexts[votedSlot].text = voteCounts[votedSlot].ToString();
        hasVoted[p] = true;
    }

    //call this from game manager to start voting
    public void StartVoting()
    {
        //Randomly slect 4 NPcs
        List<Sprite> pool = new List<Sprite>(npcPortraits);
        Shuffle(pool);

        for(int i = 0; i < 4; i++)
        {

            npcImages[i].sprite = pool[i];
            voteTexts[i].text = "0";
            voteCounts[i] = 0;
        }

        // Reset players (start all on slot 0)
        for (int p = 0; p < 4; p++)
        { 
            playerSelections[p] = 0;
            hasVoted[p] = false;
            for (int s = 0; s < 4; s++)
            {
                playerHighlights[p][s].gameObject.SetActive(false);

            }

            playerHighlights[p][0].gameObject.SetActive(true); // Show initial highlight

        }

        voteTimer = voteTime;
        votingActive = true;
        votingCanvas.gameObject.SetActive(true);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }
    private void EndVote()
    {
        votingActive = false;


        // Auto vote for non-voters based on their current highlight
        for (int p = 0; p < 4; p++)
        {
            if (!hasVoted[p])
            {
                CastVote(p); 
            }
        }
        //Find winner
        int winner = 0;
        for (int i = 1; i < 4; i++)
        {
            if (voteCounts[i] > voteCounts[winner])
            {
                winner = i;
            }
            Debug.Log($"NPC {i + 1} won by receiving {voteCounts[i]} votes.");

            // Show results for 3 seconds, the hide
            StartCoroutine(ShowResults(3f));
        }
    }

    private IEnumerator ShowResults(float delay)
    {
       
        yield return new WaitForSeconds(delay);
        votingCanvas.gameObject.SetActive(false);
    }

}
