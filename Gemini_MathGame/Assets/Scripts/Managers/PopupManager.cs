using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    [SerializeField] private PopupBase[] _popups;
    private Dictionary<string, PopupBase> _popupsDictionary = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var popup in _popups)
        {
            _popupsDictionary[popup.transform.name] = popup;
        }
    }

    public async UniTask ShowPopup(string popupName, Dictionary<string, object> popupContext = default, Action<Dictionary<string, object>> onClose = default)
    {
        if (_popupsDictionary.TryGetValue(popupName, out var popup))
        {
            if (onClose != null)
            {
                popup.OnClose += onClose;
            }

            await popup.Show(popupContext, onClose);
        }
        else
        {
            Debug.LogError($"Popup with name {popupName} not found.");
        }
    }
}
