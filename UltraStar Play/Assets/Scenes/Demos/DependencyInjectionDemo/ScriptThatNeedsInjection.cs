﻿using UniInject;
using UnityEngine;
using static UniInject.UniInject;

// Ignore warnings about unassigned fields.
// Their values are injected, but this is not visible to the compiler.
#pragma warning disable CS0649

public class ScriptThatNeedsInjection : MonoBehaviour
{
    // Inject field
    [Inject]
    private SettingsManager settingsManager;

    // Inject property
    [Inject]
    private Settings Settings { get; set; }

    // Inject field using a specific key instead of the type.
    [Inject(key = "author")]
    private string nameOfAuthor { get; set; }

    // Inject field via GetComponentInChildren
    [InjectComponent(GetComponentMethods.GetComponentInChildren)]
    private ChildOfScriptThatNeedsInjection child;

    // Inject property via GetComponentInParent
    [InjectComponent(GetComponentMethods.GetComponentInParent)]
    private ParentOfScriptThatNeedsInjection Parent { get; set; }

    // Inject readonly field via GetComponentInParent
    [InjectComponent(GetComponentMethods.GetComponentInParent)]
    private readonly OtherComponentOfScriptThatNeedsInjection siblingComponent;

    // Inject readonly property via FindObjectOfType
    [InjectComponent(GetComponentMethods.FindObjectOfType)]
    private readonly Canvas canvas;

    // The instance of this field is created during injection. And the new instance itself is injected.
    [Inject]
    private IDependencyInjectionDemoInterface demoInterfaceInstance1;

    [Inject]
    private IDependencyInjectionDemoInterface demoInterfaceInstance2;

    [Inject]
    private IDependencyInjectionDemoInterfaceWithConstructorParameters demoInterfaceInstanceWithConstructorParameters;

    // This field is set in a method via method injection
    private string methodInjectionField;

    [Inject]
    private void SetMethodInjectionField([InjectionKey("personWithAge")] string personWithAge, int age)
    {
        this.methodInjectionField = $"{personWithAge} is {age} years old";
    }

    void Start()
    {
        Debug.Log("SettingsManager: " + settingsManager);
        Debug.Log("Settings: " + Settings);

        Debug.Log("Parent: " + Parent);
        Debug.Log("Child: " + child);
        Debug.Log("Sibling Component: " + siblingComponent);

        Debug.Log("Canvas: " + canvas);

        Debug.Log("Author: " + nameOfAuthor);

        Debug.Log("Field from method injection:" + methodInjectionField);

        Debug.Log("Instance of an interface (field 1):" + demoInterfaceInstance1.GetGreeting());
        Debug.Log("Instance of an interface (field 2):" + demoInterfaceInstance2.GetGreeting());

        Debug.Log("Instance of an interface with constructor parameters:" + demoInterfaceInstanceWithConstructorParameters.GetByeBye());

        Debug.Log("An int: " + GlobalInjector.GetInstance<int>());
        Debug.Log("An instance of an interface: " + GlobalInjector.GetInstance<IDependencyInjectionDemoInterface>());
    }
}