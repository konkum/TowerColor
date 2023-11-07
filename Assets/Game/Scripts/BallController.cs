using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using Joywire;
using Joywire.Core;
public class BallController : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private LayerMask brickLayer;
    [SerializeField] private ParticleSystem explosion;
    private Collider _collider;
    private Rigidbody _rigidBody;

    private Color _color;
    private bool _isColliding = false;
    private bool _isBomb = false;
    private Tween _tween;
    public Tween Tween;
    public Rigidbody Rigidbody
    {
        get => _rigidBody;
    }
    public Collider Collider
    {
        get => _collider;
    }
    public bool IsBomb
    {
        get => _isBomb;
    }
    private IPlayerProgression _playerProgression;
    private GraphicLoader _graphicLoader;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _tween = this.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
    public void InitializedBomb()
    {
        _isBomb = true;
        _color = Color.black;
    }
    public void Initialized()
    {
        ThirdParties.Find<IPlayerProgression>(out _playerProgression);
        _graphicLoader = GetComponent<GraphicLoader>();
        _graphicLoader.OnGraphicRefresh += OnGraphicRefresh;
        GameEvent.OnGraphicReloadRequest += OnGraphicReloadRequestCallBack;
        OnGraphicReloadRequestCallBack();
    }
    public void SetColor(Color color)
    {
        meshRenderer.material.SetColor("_Color", color);
        _color = color;
    }
    private void OnCollisionEnter(Collision other)
    {
        this.gameObject.SetActive(false);
        if (other.collider.TryGetComponent<BrickController>(out var brick))
        {
            if (_isColliding)
            {
                return;
            }
            if (_isBomb)
            {
                Detonate();
                return;
            }
            if (_color != brick.Color)
            {
                return;
            }
            if (!brick.IsInteractable)
            {
                return;
            }
            _isColliding = true;
            brick.BrickNearThisBrick();
            brick.DropedBrick();
        }
    }
    private void Detonate()
    {
        Vector3 explosionPosition = new Vector3(0, this.transform.localPosition.y, 0);
        var explode = Instantiate(explosion, explosionPosition, Quaternion.identity);
        explode.Emit(300);
        Collider[] colliders = new Collider[100];
        Physics.OverlapSphereNonAlloc(explosionPosition, 10, colliders, brickLayer);
        foreach (Collider hit in colliders)
        {
            if (hit != null)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(50, explosionPosition, 10, 1, ForceMode.Impulse);
                }
            }
        }
    }
    private void OnGraphicReloadRequestCallBack()
    {
        var itemId = _playerProgression.PlayerProfile.GetPlayerCustom("BALL_TYPE").itemId;
        _graphicLoader.LoadGraphic(itemId);
    }
    private void OnGraphicRefresh(GameObject gameObject)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        SetColor(_color);
    }
    private void OnCollisionExit(Collision other)
    {
        _isColliding = false;
    }
    private void OnDestroy()
    {
        _tween.Kill();
    }
}
