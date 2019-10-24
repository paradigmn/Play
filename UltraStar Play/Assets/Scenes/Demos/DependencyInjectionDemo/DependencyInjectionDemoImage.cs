using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DependencyInjectionDemoImage : MonoBehaviour
{
    public Text text;

    private I18NManager i18nManager;
    private string injectedName;

    [Inject]
    public void InitDependencies(I18NManager i18nManager, string injectedName)
    {
        this.i18nManager = i18nManager;
        this.injectedName = injectedName;
    }

    void Start()
    {
        Dictionary<string, string> translationArguments = DictionaryUtils.OfValues("name", injectedName);
        string translatedText = i18nManager.GetTranslation("i18n_demo_scene.hello", translationArguments);
        text.text = translatedText;
    }
}
