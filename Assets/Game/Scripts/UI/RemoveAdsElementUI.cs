using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RemoveAdsElementUI : UIScript
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject mainObject;
    public override void Initialized()
    {
        base.Initialized();
        closeBtn.onClick.AddListener(Hide);
    }
    public override void Show()
    {
        base.Show();
        mainObject.transform.localScale = Vector3.zero;
        mainObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public override void Hide()
    {
        mainObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => base.Hide());
    }
}
