using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SingingResultsSceneKeyboardController : MonoBehaviour
{

    private const KeyCode ContinueToNextSceneShortcut = KeyCode.Return;
    private const KeyCode ContinueToNextSceneShortcut2 = KeyCode.Escape;

    private SingingResultsSceneController controller;

    [Inject]
    public void InitDependencies(SingingResultsSceneController controller)
    {
        this.controller = controller;
    }

    void Update()
    {
        if (Input.GetKeyUp(ContinueToNextSceneShortcut) || Input.GetKeyUp(ContinueToNextSceneShortcut2))
        {
            controller.FinishScene();
        }
    }
}
