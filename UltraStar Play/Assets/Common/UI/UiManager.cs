﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniInject;
using UniRx;

// Disable warning about fields that are never assigned, their values are injected.
#pragma warning disable CS0649

public class UiManager : MonoBehaviour, INeedInjection
{
    public static UiManager Instance
    {
        get
        {
            return GameObjectUtils.FindComponentWithTag<UiManager>("UiManager");
        }
    }

    private readonly List<RectTransform> debugPoints = new List<RectTransform>();

    [InjectedInInspector]
    public WarningDialog warningDialogPrefab;

    [InjectedInInspector]
    public QuestionDialog questionDialogPrefab;

    [InjectedInInspector]
    public Notification notificationPrefab;

    [InjectedInInspector]
    public RectTransform debugPositionIndicatorPrefab;

    [InjectedInInspector]
    public ContextMenu contextMenuPrefab;

    [InjectedInInspector]
    public Tooltip tooltipPrefab;

    [InjectedInInspector]
    public ShowFps showFpsPrefab;

    private readonly Subject<Vector3> mousePositionChangeEventStream = new Subject<Vector3>();
    public IObservable<Vector3> MousePositionChangeEventStream => mousePositionChangeEventStream;

    public bool DialogOpen => dialogs.Count > 0;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private float notificationHeightInPixels;
    private float notificationWidthInPixels;

    [Inject]
    private Injector injector;

    private readonly List<Notification> notifications = new List<Notification>();
    private readonly List<Dialog> dialogs = new List<Dialog>();

    private Vector3 lastMousePosition;

    private ShowFps showFpsInstance;

    void Awake()
    {
        LeanTween.init(800);
    }

    void Start()
    {
        notificationHeightInPixels = notificationPrefab.GetComponent<RectTransform>().rect.height;
        notificationWidthInPixels = notificationPrefab.GetComponent<RectTransform>().rect.width;

        if (SettingsManager.Instance.Settings.DeveloperSettings.showFps)
        {
            CreateShowFpsInstance();
        }
    }

    void Update()
    {
        if (lastMousePosition != Input.mousePosition)
        {
            mousePositionChangeEventStream.OnNext(Input.mousePosition);
        }
        lastMousePosition = Input.mousePosition;
    }

    public void CreateShowFpsInstance()
    {
        if (showFpsInstance != null)
        {
            return;
        }

        showFpsInstance = Instantiate(showFpsPrefab, CanvasUtils.FindCanvas().GetComponent<RectTransform>());
        // Move to front
        showFpsInstance.transform.SetAsLastSibling();
        showFpsInstance.transform.position = new Vector3(20, 20, 0);
    }

    public void DestroyShowFpsInstance()
    {
        if (showFpsInstance != null)
        {
            Destroy(showFpsInstance);
        }
    }

    public WarningDialog CreateWarningDialog(string title, string message)
    {
        FindCanvas();

        WarningDialog warningDialog = Instantiate(warningDialogPrefab, canvas.transform);
        injector.Inject(warningDialog);
        warningDialog.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        if (title != null)
        {
            warningDialog.Title = title;
        }
        if (message != null)
        {
            warningDialog.Message = message;
        }

        dialogs.Add(warningDialog);
        return warningDialog;
    }

    public QuestionDialog CreateQuestionDialog(string title, string message)
    {
        FindCanvas();

        QuestionDialog questionDialog = Instantiate(questionDialogPrefab, canvas.transform);
        injector.Inject(questionDialog);
        questionDialog.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        if (title != null)
        {
            questionDialog.Title = title;
        }
        if (message != null)
        {
            questionDialog.Message = message;
        }

        dialogs.Add(questionDialog);
        return questionDialog;
    }

    public Notification CreateNotification(string message)
    {
        FindCanvas();

        Notification notification = Instantiate(notificationPrefab, canvas.transform);
        injector.Inject(notification);
        notification.SetText(message);
        PositionNotification(notification, notifications.Count);

        notifications.Add(notification);
        return notification;
    }

    public void OnDialogClosed(Dialog dialog)
    {
        dialogs.Remove(dialog);
    }

    public Notification CreateNotification(string message, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return CreateNotification($"<color=\"#{hexColor}\">{message}</color>");
    }

    private void FindCanvas()
    {
        if (canvas == null)
        {
            canvas = CanvasUtils.FindCanvas();
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    private void PositionNotification(Notification notification, int index)
    {
        float anchoredHeight = notificationHeightInPixels / canvasRectTransform.rect.height;
        float anchoredWidth = notificationWidthInPixels / canvasRectTransform.rect.width;
        float x = 0;
        float y = (index * notificationHeightInPixels) / canvasRectTransform.rect.height;
        notification.RectTransform.anchorMin = new Vector2(x, y);
        notification.RectTransform.anchorMax = new Vector2(x + anchoredWidth, y + anchoredHeight);
        notification.RectTransform.sizeDelta = Vector2.zero;
        notification.RectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnNotificationDestroyed(Notification notification)
    {
        notifications.Remove(notification);
        int index = 0;
        foreach (Notification n in notifications)
        {
            PositionNotification(n, index);
            index++;
        }
    }

    public void DestroyAllDebugPoints()
    {
        foreach (RectTransform debugPoint in debugPoints)
        {
            GameObject.Destroy(debugPoint.gameObject);
        }
        debugPoints.Clear();
    }

    public RectTransform CreateDebugPoint(RectTransform parent = null)
    {
        if (parent == null)
        {
            parent = CanvasUtils.FindCanvas().GetComponent<RectTransform>();
        }
        RectTransform debugPoint = GameObject.Instantiate(debugPositionIndicatorPrefab, parent);
        debugPoints.Add(debugPoint);
        return debugPoint;
    }
}
