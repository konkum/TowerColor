using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Joywire;
public class KeyController : MonoBehaviour
{
    private Joywire.Core.IPlayerProgression playerProgression;
    private Tween _tween;
    private void Start()
    {
        _tween = this.transform.DORotate(new Vector3(0, 90, 0), 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BallController>(out var ball))
        {
            KeyCollected().Play().OnComplete(() => this.gameObject.SetActive(false));
            Joywire.ThirdParties.Find<Joywire.Core.IPlayerProgression>(out playerProgression);
            var currentKeys = playerProgression.PlayerProfile.GetCurrency("currency_01").amount;
            if (currentKeys >= 3)
            {
                return;
            }
            var summary = currentKeys + 1;
            ThirdParties.Find<IMissionController>(out var missionController);
            missionController.UpdateMission(MissionType.CollectKeys, 1);
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_01", summary);
        }
    }
    private Sequence KeyCollected()
    {
        _tween.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(new Vector3(0, 90, 0), 0.1f).SetEase(Ease.Linear).SetLoops(10, LoopType.Incremental));
        sequence.Join(transform.DOMoveY(this.transform.position.y + 2f, 1f).SetEase(Ease.Linear));

        return sequence;
    }
    private void OnDestroy()
    {
        _tween.Kill();
    }
}
