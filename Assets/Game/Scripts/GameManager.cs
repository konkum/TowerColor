using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Joywire;
using Joywire.Core;
using Joywire.Monetization;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    [SerializeField] private GameObject ballHolder;
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private BallController bomb;
    [SerializeField] private LayerMask brickLayer;
    private List<BallController> ballList = new List<BallController>();
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TowerController tower;
    [SerializeField] private List<ParticleSystem> fireworks;
    [SerializeField] private KeyController key;

    public TowerController Tower
    {
        get => tower;
        set => tower = value;
    }

    private bool _isGameStart = false;
    private bool _isEndGame = false;
    private float _downClickTime;
    private float _clickDeltaTime = 0.13f;
    public bool IsGameStart
    {
        get => _isGameStart;
        set
        {
            if (value)
            {
                cameraController.CurrentHeight = System.Convert.ToInt32((float)tower.Bricks.Count / 20f);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(cameraController.RotateAnim());
                sequence.Join(cameraController.Move());
                sequence.Play().OnComplete(() =>
                {
                    _isGameStart = true;
                    GameEvent.OnGameStart?.Invoke();
                    GameEvent.UpdateBallCount?.Invoke(ballList.Count);
                    cameraController.SetTrigger();
                    ShowNextBall();
                });
            }
        }
    }
    public bool IsEndGame
    {
        get => _isEndGame;
        set => _isEndGame = value;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        ThirdParties.Find<IPlayerProgression>(out var playerProgression);
        if (playerProgression.PlayerProfile.PlayerProgress.CurrentID == "Level 2" || playerProgression.PlayerProfile.PlayerProgress.CurrentID == "Level 4")
        {
            tower.Spawn2();
        }
        else
        {
            tower.Spawn();
        }
        for (int i = 0; i < 20; i++)
        {
            var ball = Instantiate(ballPrefab, ballHolder.transform);
            var rand = UnityEngine.Random.Range(0, tower.Colors.Count);
            ball.Initialized();
            ball.SetColor(tower.Colors[rand]);
            ballList.Add(ball);
        }
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        key.transform.position = new Vector3(Mathf.Sin(angle) * 3.5f, UnityEngine.Random.Range(2.1f, System.Convert.ToInt32((float)tower.Bricks.Count / 20f)), Mathf.Cos(angle) * 3.5f);
        GameEvent.OnEndGame += OnEndGameCallBack;
        GameEvent.OnAddAditionalBall += SpawnAditionalBall;
        ThirdParties.Find<IAdmobController>(out var admobController);
        admobController.ShowBanner(true);
    }
    private void Update()
    {
        if (!_isGameStart)
        {
            return;
        }
        if (_isEndGame)
        {
            return;
        }
        cameraController.MoveCameraToHeight(System.Convert.ToInt32((float)tower.Bricks.Count / 20f));
        if (Input.GetMouseButtonDown(0))
        {
            _downClickTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - _downClickTime <= _clickDeltaTime)
            {
                Raycast();
            }
        }
    }
    private void Raycast()
    {
        RaycastHit hit;
        Ray ray = Helpers.Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, brickLayer))
        {
            ballList[0].Tween.Kill();
            ballList[0].transform.rotation = Quaternion.identity;
            ballList[0].transform.localPosition = new Vector3(0, -1.5f, 0);
            ballList[0].transform.parent = null;
            ballList[0].Collider.enabled = true;
            ballList[0].Rigidbody.useGravity = true;
            ballList[0].Rigidbody.velocity = calculateBestThrowSpeed(ballList[0].transform.position, hit.point, 0.8f);
            ballList.RemoveAt(0);
            GameEvent.UpdateBallCount?.Invoke(ballList.Count);
            ShowNextBall();
            if (tower.Bricks.Count <= 50)
            {
                return;
            }
            if (ballList.Count <= 0)
            {
                _isEndGame = true;
                StartCoroutine(WaitToWin());
            }
        }
    }
    private IEnumerator WaitToWin()
    {
        yield return Helpers.GetWait(3f);
        if (tower.Bricks.Count <= 70)
        {
            GameEvent.OnEndGame?.Invoke(true);
        }
        else
        {
            GameEvent.OnEndGame?.Invoke(false);
        }
    }
    private void ShowNextBall()
    {
        if (ballList.Count == 0)
        {
            return;
        }
        ballList[0].gameObject.SetActive(true);
        ballList[0].transform.position = cameraController.PresentBallTransform.position;
        if (ballList.Count >= 2)
        {
            ballList[1].gameObject.SetActive(true);
            ballList[1].transform.position = cameraController.NextBallTransform.position;
        }
    }
    private void OnEndGameCallBack(bool isWin)
    {
        _isEndGame = true;
        for (int i = 0; i < ballList.Count; i++)
        {
            if (ballList[i].gameObject.activeInHierarchy)
            {
                ballList[i].gameObject.SetActive(false);
            }
        }
    }
    private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        // calculate vectors
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // calculate xz and y
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float t = timeToTarget;
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;

        Vector3 result = toTargetXZ.normalized;
        result *= v0xz;
        result.y = v0y;

        return result;
    }
    public void PlayFireWorks()
    {
        for (int i = 0; i < fireworks.Count; i++)
        {
            fireworks[i].Emit(100);
        }
    }
    private void SpawnAditionalBall()
    {
        List<Color> currentColor = new List<Color>();
        for (int i = 0; i < tower.Bricks.Count; i++)
        {
            if (!currentColor.Contains(tower.Bricks[i].Color))
            {
                currentColor.Add(tower.Bricks[i].Color);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            var ball = Instantiate(ballPrefab, ballHolder.transform);
            var rand = UnityEngine.Random.Range(0, currentColor.Count);
            ball.Initialized();
            ball.SetColor(currentColor[rand]);
            ballList.Add(ball);
        }
        ShowNextBall();
        GameEvent.UpdateBallCount?.Invoke(ballList.Count);
    }
    public void ReplaceBallWithBomb()
    {
        for (int i = 0; i < ballList.Count; i++)
        {
            if (!ballList[i].IsBomb)
            {
                var bomb = Instantiate(this.bomb, ballList[i].transform.position, Quaternion.identity);
                bomb.transform.SetParent(Helpers.Camera.transform);
                bomb.InitializedBomb();
                ballList[i].gameObject.SetActive(false);
                ballList[i] = bomb;
                return;
            }
        }
    }
    private void OnDestroy()
    {
        GameEvent.OnGameStart = null;
        GameEvent.OnEndGame = null;
        GameEvent.UpdateBallCount = null;
        GameEvent.OnMoneyUpdate = null;
        GameEvent.OnGraphicReloadRequest = null;
        GameEvent.OnShowRewardUI = null;
        GameEvent.OnShopItemUpdate = null;
        GameEvent.OnAddAditionalBall = null;
        GameEvent.OnBombSliderUpdate = null;
        GameEvent.OnMissionUpdate = null;
        GameEvent.OnUpdateShop = null;
    }
}
public static class GameEvent
{
    public static Action OnGameStart;
    public static Action<bool> OnEndGame;
    public static Action<int> UpdateBallCount;
    public static Action<int> OnMoneyUpdate;
    public static Action OnGraphicReloadRequest;
    public static Action OnShowRewardUI;
    public static Action<string> OnShopItemUpdate;
    public static Action OnAddAditionalBall;
    public static Action<float> OnBombSliderUpdate;
    public static Action<int> OnMissionUpdate;
    public static Action<string> OnUpdateShop;
    public static Action ReInitializedMission;
}
