using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DynamicInjectionContext : MonoInstaller
{

    public string text;
    private string lastText;

    public override void InstallBindings()
    {
    }

    void Update()
    {
        // Rebind the string
        if (lastText != text)
        {
            lastText = text;
            Rebind();
        }
    }

    private void Rebind()
    {
        Debug.Log("Rebinding text");
        Container.UnbindId<string>("theDynamicInjectionId");
        Container.BindInstance(text).WithId("theDynamicInjectionId");
        DynamicInjectionUser dynamicInjectionUser = FindObjectOfType<DynamicInjectionUser>();
        Container.Inject(dynamicInjectionUser);
    }
}
