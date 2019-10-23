using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DependencyInjectionDemoSceneContext : MonoInstaller
{
    public override void InstallBindings()
    {
        // Global bindings for the scene
        Container.BindInstance(I18NManager.Instance);
        Container.BindInstance("the-SceneContext-injected-this").WithId("theInjectedNameId");
    }
}
