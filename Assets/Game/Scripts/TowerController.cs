using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TowerController : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private BrickController brickPrefab;
    [SerializeField] private int numberOfSpawn = 4;
    [SerializeField] private float radius = 3f;
    private Texture2D _levelTexture;
    private List<BrickController> bricks = new List<BrickController>();
    private List<Color> colors = new List<Color>();
    private int _activeCount = 0;
    public List<BrickController> Bricks => bricks;
    public List<Color> Colors => colors;
    private bool _playOnce = false;
    private IMissionController missionController;
    private int _countInactiveBricks = 0;
    public void Spawn()
    {
        Joywire.ThirdParties.Find<IMissionController>(out missionController);
        Joywire.ThirdParties.Find<Joywire.Core.IPlayerProgression>(out var playerProgression);
        Joywire.ThirdParties.Find<Joywire.Core.IResourceLoader>(out var resourceLoader);
        resourceLoader.LoadTexture(playerProgression.PlayerProfile.PlayerProgress.CurrentID, out var image);
        _levelTexture = image;
        float spawnShift = 0f;
        float heightShift = 0f;
        for (int j = 0; j < 20; j++)
        {
            for (int i = 0; i < numberOfSpawn; i++)
            {
                float angle = i * Mathf.PI * 2f / numberOfSpawn + spawnShift;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, this.transform.position.y + heightShift, Mathf.Sin(angle) * radius);
                var brick = Instantiate(brickPrefab, newPos, Quaternion.identity);
                brick.transform.SetParent(this.transform);
                brick.Color = _levelTexture.GetPixel(numberOfSpawn - i, j);
                if (!colors.Contains(brick.Color))
                {
                    colors.Add(brick.Color);
                }
                if (j >= 12)
                {
                    _activeCount++;
                    brick.IsInteractable = true;
                }
                else
                {
                    brick.IsInteractable = false;
                }
                brick.Tower = this;
                bricks.Add(brick);
            }
            heightShift += brickPrefab.GetComponent<MeshRenderer>().bounds.extents.y * 2;
            spawnShift += 30f;
        }
    }
    public void Spawn2()
    {
        Joywire.ThirdParties.Find<IMissionController>(out missionController);
        Joywire.ThirdParties.Find<Joywire.Core.IPlayerProgression>(out var playerProgression);
        Joywire.ThirdParties.Find<Joywire.Core.IResourceLoader>(out var resourceLoader);
        resourceLoader.LoadTexture(playerProgression.PlayerProfile.PlayerProgress.CurrentID, out var image);
        _levelTexture = image;
        float spawnShift = 0f;
        float heightShift = 0f;
        for (int j = 0; j < 20; j++)
        {
            var squareSpawn = Instantiate(squarePrefab, this.transform.position + new Vector3(0, heightShift, 0), Quaternion.identity);
            squareSpawn.transform.SetParent(this.transform);
            squareSpawn.transform.rotation = Quaternion.Euler(0, spawnShift, 0);
            for (int i = 0; i < 16; i++)
            {
                var brick = squareSpawn.transform.GetChild(i).GetComponent<BrickController>();
                brick.Color = _levelTexture.GetPixel(numberOfSpawn - i, j);
                if (!colors.Contains(brick.Color))
                {
                    colors.Add(brick.Color);
                }
                if (j >= 12)
                {
                    _activeCount++;
                    brick.IsInteractable = true;
                }
                else
                {
                    brick.IsInteractable = false;
                }
                brick.Tower = this;
                bricks.Add(brick);
            }
            heightShift += brickPrefab.GetComponent<MeshRenderer>().bounds.extents.y * 2;
            spawnShift += 5f;
        }
    }
    public void UpdateTower()
    {
        for (int i = bricks.Count - 1; i >= 0; i--)
        {
            if (bricks[i].IsInteractable)
            {
                continue;
            }
            bricks[i].IsInteractable = true;
            _activeCount++;
            if (_activeCount >= 128)
            {
                break;
            }
        }
        Vibrator.Vibrate();
    }
    public void RemoveBricks()
    {
        int countBeforeRemove = bricks.Count;
        bricks.RemoveAll(RemoveInActive);
        missionController.UpdateMission(MissionType.ExplodeBalls, _countInactiveBricks);
        _countInactiveBricks = 0;
        bricks.RemoveAll(item => item.transform.position.y <= 0f);
        int countAfterRemove = bricks.Count;
        GameEvent.OnBombSliderUpdate?.Invoke(countBeforeRemove - countAfterRemove);
        _activeCount -= countBeforeRemove - countAfterRemove;
        if (_activeCount <= 112)
        {
            UpdateTower();
        }
        if (bricks.Count <= 70 && !_playOnce)
        {
            _playOnce = true;
            GameManager.Instance.PlayFireWorks();
            GameManager.Instance.IsEndGame = true;
            StartCoroutine(Wait());
        }
    }
    private bool RemoveInActive(BrickController brick)
    {
        if (!brick.gameObject.activeInHierarchy)
        {
            _countInactiveBricks++;
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator Wait()
    {
        yield return Helpers.GetWait(3f);
        GameEvent.OnEndGame?.Invoke(true);
    }
}
