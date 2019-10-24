using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CommonSceneObjectsBinder : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(I18NManager.Instance);
        Container.BindInstance(ThemeManager.Instance);
        Container.BindInstance(ApplicationManager.Instance);
        Container.BindInstance(PlayerProfileManager.Instance);
        Container.BindInstance(SongMetaManager.Instance);
        Container.BindInstance(SceneNavigator.Instance);
    }
}
