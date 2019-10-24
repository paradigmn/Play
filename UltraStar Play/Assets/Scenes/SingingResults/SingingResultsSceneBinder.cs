using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SingingResultsSceneBinder : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(SingingResultsSceneController.Instance);
    }
}
