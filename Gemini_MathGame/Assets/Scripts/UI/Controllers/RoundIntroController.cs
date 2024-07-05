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

        if (PlayerPrefs.GetInt(Prefs.DontShowInfoPopupAgain) != 1)
        {
            await PopupManager.Instance.ShowPopup(PopupNames.RoundStartInfoPopup, new Dictionary<string, object>
            {
                [GameParams.solution] = solutionTarget
            }, context =>
            {
                ready = true;
            });
        }
        else
        {
            ready = true;
        }
        
        await UniTask.WaitUntil(() => ready);
        
        await _countdownController.Async_StartCountdown(
            new Queue<string>(new[] { "3", "2", "1", "GO!" }),
            800
        );
    }
}
