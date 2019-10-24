using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SingingResultsSceneController : MonoBehaviour
{
    public Text songLabel;

    public GameObject onePlayerLayout;
    public GameObject twoPlayerLayout;

    private SingingResultsSceneData sceneData;

    public static SingingResultsSceneController Instance
    {
        get
        {
            return FindObjectOfType<SingingResultsSceneController>();
        }
    }

    private SceneNavigator sceneNavigator;
    private SongMetaManager songMetaManager;
    private PlayerProfileManager playerProfileManager;

    [Inject]
    public void InitDependencies(SceneNavigator sceneNavigator, SongMetaManager songMetaManager, PlayerProfileManager playerProfileManager)
    {
        this.sceneNavigator = sceneNavigator;
        this.songMetaManager = songMetaManager;
        this.playerProfileManager = playerProfileManager;
    }

    void Start()
    {
        sceneData = sceneNavigator.GetSceneData<SingingResultsSceneData>(CreateDefaultSceneData());
        SelectLayout();
        FillLayout();
    }

    private void FillLayout()
    {
        SongMeta songMeta = sceneData.SongMeta;
        string titleText = (string.IsNullOrEmpty(songMeta.Title)) ? "" : songMeta.Title;
        string artistText = (string.IsNullOrEmpty(songMeta.Artist)) ? "" : " - " + songMeta.Artist;
        songLabel.text = titleText + artistText;

        int i = 0;
        GameObject selectedLayout = GetSelectedLayout();
        foreach (PlayerProfile playerProfile in sceneData.PlayerProfiles)
        {
            SingingResultsSceneData.PlayerScoreData playerScoreData = sceneData.GetPlayerScores(playerProfile);
            SingingResultsPlayerUiController[] uiControllers = selectedLayout.GetComponentsInChildren<SingingResultsPlayerUiController>();
            if (i < uiControllers.Length)
            {
                uiControllers[i].Init(playerProfile, playerScoreData);
            }
            i++;
        }
    }

    private void SelectLayout()
    {
        int playerCount = sceneData.PlayerProfiles.Count;
        List<GameObject> layouts = new List<GameObject>();
        layouts.Add(onePlayerLayout);
        layouts.Add(twoPlayerLayout);

        GameObject selectedLayout = GetSelectedLayout();
        foreach (GameObject layout in layouts)
        {
            layout.SetActive(layout == selectedLayout);
        }
    }

    private GameObject GetSelectedLayout()
    {
        int playerCount = sceneData.PlayerProfiles.Count;
        if (playerCount == 2)
        {
            return twoPlayerLayout;
        }
        return onePlayerLayout;
    }

    private SingingResultsSceneData CreateDefaultSceneData()
    {
        SingingResultsSceneData data = new SingingResultsSceneData();
        data.SongMeta = songMetaManager.SongMetas[0];

        SingingResultsSceneData.PlayerScoreData playerScoreData = new SingingResultsSceneData.PlayerScoreData();
        playerScoreData.TotalScore = 6500;
        playerScoreData.NormalNotesScore = 4000;
        playerScoreData.GoldenNotesScore = 2000;
        playerScoreData.PerfectSentenceBonusScore = 500;

        data.AddPlayerScores(playerProfileManager.PlayerProfiles[0], playerScoreData);
        return data;
    }

    public void FinishScene()
    {
        SongSelectSceneData songSelectSceneData = new SongSelectSceneData();
        songSelectSceneData.SongMeta = sceneData.SongMeta;

        sceneNavigator.LoadScene(EScene.SongSelectScene, songSelectSceneData);
    }
}
