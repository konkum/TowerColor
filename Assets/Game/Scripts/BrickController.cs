using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class BrickController : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask brickLayer;
    [SerializeField] private Color colorBlack;
    [SerializeField] private ParticleSystem effect;
    private Color _color;
    private bool _isInteractable;
    public bool IsInteractable
    {
        get => _isInteractable && gameObject.activeSelf;
        set
        {
            IsColored = value;
            rb.isKinematic = !value;
            _isInteractable = value;
        }
    }
    public Color Color
    {
        get => _color;
        set => _color = value;
    }
    public bool IsColored
    {
        set
        {
            if (value)
            {
                ColorFromConfig();
            }
            else
            {
                ColorBlack();
            }
        }
    }
    private TowerController _tower;
    public TowerController Tower
    {
        get => _tower;
        set => _tower = value;
    }
    public void ColorBlack()
    {
        meshRenderer.material.SetColor("_Color", colorBlack);
    }
    public void ColorFromConfig()
    {
        effect.startColor = _color;
        meshRenderer.material.SetColor("_Color", _color);
    }
    public void BrickNearThisBrick()
    {
        effect.transform.SetParent(null);
        effect.Emit(100);
        this.gameObject.SetActive(false);
        Collider[] hits = new Collider[8];
        int numberOfHits = Physics.OverlapSphereNonAlloc(this.transform.position, 0.9f, hits, brickLayer);
        if (numberOfHits == 0)
        {
            return;
        }
        for (int i = 0; i < numberOfHits; i++)
        {
            if (hits[i] == this.gameObject)
            {
                continue;
            }
            if (hits[i] == null)
            {
                continue;
            }
            var brick = hits[i].GetComponent<BrickController>();
            if (!brick.Color.IsEqualTo(_color))
            {
                continue;
            }
            if (!brick.IsInteractable)
            {
                continue;
            }
            brick.BrickNearThisBrick();
        }
    }
    public void DropedBrick()
    {
        if (!GameManager.Instance.IsEndGame)
        {
            SoundManager.Instance.PlaySound(Sound.BlockDestroy);
        }
        _tower.RemoveBricks();
    }
}
