using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    [SerializeField] private EventTrigger inputTrigger;
    [SerializeField] private Transform presentBallLocation;
    [SerializeField] private Transform nextBallLocation;

    private int _currentHeight;
    public int CurrentHeight
    {
        get => _currentHeight;
        set => _currentHeight = value;
    }
    public Transform PresentBallTransform
    {
        get => presentBallLocation;
    }
    public Transform NextBallTransform
    {
        get => nextBallLocation;
    }
    public void MoveCameraToHeight(int height)
    {
        _currentHeight = height;
        int result = Mathf.Clamp(height - 7, 0, 1000);
        transform.position = Vector3.Lerp(transform.position, new Vector3(0, result, 0), 0.5f * Time.deltaTime);
    }
    public void SetTrigger()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        inputTrigger.triggers.Add(entry);
    }
    private void OnDrag(PointerEventData data)
    {
        transform.Rotate(0, data.delta.x * 0.2f, 0);
    }
    public Tween RotateAnim()
    {
        return transform.DORotate(new Vector3(0, 360f, 0f), 3f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear);
    }
    public Tween Move()
    {
        return transform.DOMoveY(_currentHeight - 7, 3f);
    }
}
