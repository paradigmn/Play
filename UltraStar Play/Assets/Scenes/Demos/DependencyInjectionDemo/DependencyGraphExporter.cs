using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

public class DependencyGraphExporter : MonoBehaviour
{
    public string outputFile = "./Assets/Scenes/Demos/DependencyInjectionDemo/object-graph.txt";

    void Start()
    {
        // Somehow, the injected string does not appear in the exported file,
        // although it is part of the includedTypes list.
        // Anyway, the exported file shows that the DependencyInjectionDemoImage
        // uses an injected version of I18NManager.
        //
        // You can render the exported, textual graphviz file online: http://www.webgraphviz.com/
        IEnumerable<Type> excludedTypes = new List<Type>();
        Type[] includedTypes = new Type[] { typeof(string), typeof(DependencyInjectionDemoImage) };
        SceneContext sceneContext = FindObjectOfType<SceneContext>();
        ObjectGraphVisualizer.OutputObjectGraphToFile(sceneContext.Container, outputFile, excludedTypes, includedTypes);
    }
}
