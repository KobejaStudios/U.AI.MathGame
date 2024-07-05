using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RoundIntroController : MonoBehaviour
{
    [SerializeField] private CountdownController _countdownController;

    public async UniTask StartRoundIntroFlow(int solutionTarget)
    {
        var ready = false;
        Debug.Log("Check PlayerPrefs for DontShowInfoPopupAgain");

        if (PlayerPrefs.GetInt(Prefs.DontShowInfoPopupAgain) != 1)
        {
            Debug.Log("Showing popup");
            await PopupManager.Instance.ShowPopup(PopupNames.RoundStartInfoPopup, new Dictionary<string, object>
            {
                [GameParams.solution] = solutionTarget
            }, context =>
            {
                ready = true;
                Debug.Log("Popup callback invoked");
            });
        }
        else
        {
            ready = true;
            Debug.Log("Popup not shown, setting ready to true");
        }

        Debug.Log("Waiting until ready");
        await UniTask.WaitUntil(() => ready);
        Debug.Log("Ready, starting countdown");
        
        await _countdownController.Async_StartCountdown(
            new Queue<string>(new[] { "3", "2", "1", "GO!" }),
            800
        );
    }
}
