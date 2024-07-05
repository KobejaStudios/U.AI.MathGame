using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public Dictionary<string, object> CurrentContext { get; private set; }

    public event Action<Dictionary<string, object>> OnClose;
    public event Action<Dictionary<string, object>> OnContextSet;

    private Action<Dictionary<string, object>> _onCloseCallback;

    private bool _isClosing = false;

    public async UniTask Show(Dictionary<string, object> context, Action<Dictionary<string, object>> onCloseCallback = default)
    {
        gameObject.SetActive(true);
        CurrentContext = context;
        OnContextSet?.Invoke(context);
        ShowImpl();

        if (onCloseCallback != null)
        {
            _onCloseCallback = onCloseCallback;
            OnClose += _onCloseCallback;
        }
        
        //await ShowTransition();
    }

    private void OnDestroy()
    {
        InvokeCloseCallbacks();
    }
    
    protected virtual void ShowImpl() { }

    // Mostly intended for easy hook-up to Unity events. Prefer the normal Close() when invoking from the code
    public void CloseAsVoid()
    {
        Close().Forget();
    }

    public async virtual UniTask Close()
    {
        InvokeCloseCallbacks();
        EventManager.RaiseEvent(GameEvents.PopupClosed, new Dictionary<string, object>
        {
            [GameParams.popupName] = transform.name
        });
        if (this)
        {
            gameObject.SetActive(false);
        }
    }

    // private async UniTask ShowTransition()
    // {
    //     if (!popupTransitionEffects) return;
    //     try
    //     {
    //         await popupTransitionEffects.Show();
    //     }
    //     catch (Exception e)
    //     {
    //         GameSystems.Get<IDebug>().LogException(e, PopupManager.LogCategory);
    //     }
    // }
    //
    // private async UniTask HideTransition()
    // {
    //     if (!popupTransitionEffects) return;
    //     try
    //     {
    //         await popupTransitionEffects.Hide();
    //     }
    //     catch (Exception e)
    //     {
    //         GameSystems.Get<IDebug>().LogException(e, PopupManager.LogCategory);
    //     };
    // }
    
    private void InvokeCloseCallbacks()
    {
        _isClosing = true;
        OnClose?.Invoke(CurrentContext);
        if (_onCloseCallback != null)
        {
            OnClose -= _onCloseCallback;
            _onCloseCallback = null;
        }
        OnClose = null;
    }
}