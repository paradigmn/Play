using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SongSelectSceneBinder : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(SongSelectSceneController.Instance);
    }
}
