using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DynamicInjectionUser : MonoBehaviour
{

    [Inject]
    public void SetText([InjectOptional(Id = "theDynamicInjectionId")] string newValue)
    {
        GetComponent<Text>().text = newValue;
    }
}
