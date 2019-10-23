using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingingResultsSceneKeyboardController : MonoBehaviour
{

    private const KeyCode ContinueToNextSceneShortcut = KeyCode.Return;
    private const KeyCode ContinueToNextSceneShortcut2 = KeyCode.Escape;
    private const KeyCode ContinueToNextSceneShortcut3 = KeyCode.Backspace;

    void Update()
    {
        if (Input.GetKeyUp(ContinueToNextSceneShortcut) ||
            Input.GetKeyUp(ContinueToNextSceneShortcut2) ||
            Input.GetKeyUp(ContinueToNextSceneShortcut3))
        {
            SingingResultsSceneController.Instance.FinishScene();
        }
    }
}
