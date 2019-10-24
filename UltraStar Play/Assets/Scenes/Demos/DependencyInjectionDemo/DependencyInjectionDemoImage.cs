using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DependencyInjectionDemoImage : MonoBehaviour
{
    public Text text;

    // Disable warning 0649, i.e., that the variables are never assigned
    // and will always have their default value, because this is not true.
    // The values are injected from outside.
#pragma warning disable 0649
    [Inject]
    private I18NManager i18nManager;

    [Inject(Id = "theInjectedNameId")]
    private string injectedName;
#pragma warning restore 0649

    void Start()
    {
        Dictionary<string, string> translationArguments = DictionaryUtils.OfValues("name", injectedName);
        string translatedText = i18nManager.GetTranslation("i18n_demo_scene.hello", translationArguments);
        text.text = translatedText;
    }
}
