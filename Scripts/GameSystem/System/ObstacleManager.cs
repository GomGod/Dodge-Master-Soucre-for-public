using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject bombPrefab;
    
    private readonly Queue<Pattern> _patterns = new();
    private readonly ObjectPool _laserPool = new();
    private readonly ObjectPool _bombPool = new();

    private void Start()
    {
        _laserPool.InitObjectPool(laserPrefab);
        _bombPool.InitObjectPool(bombPrefab);
    }

    public void ClearPatterns()
    {
        _patterns.Clear();
    }
    
    public void ActivateObstacleProcessor()
    {
        StopAllCoroutines();
        StartCoroutine(PatternProcessor());
        StartCoroutine(BombGenerator());
    }

    public void DisableObstacleProcessor()
    {
        StopAllCoroutines();
    }

    private IEnumerator BombGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            GenerateBomb();
        }
    }

    private void GenerateBomb()
    {
        var rx = Random.Range(1, 6);
        var ry = Random.Range(1, 6);

        var bObj = _bombPool.ActivateObject();
        var bombComponent = bObj.GetComponent<Bomb>();

        bombComponent.InitMine(new Vector2(rx, ry));
    }

    private IEnumerator PatternProcessor()
    {
        while (true)
        {
            yield return new WaitUntil(() => _patterns.Count > 0 && !GameManager.Instance.durPattern);
            var currentPattern = _patterns.Dequeue();
            GameManager.Instance.durPattern = true;
            foreach (var pInfo in currentPattern.ProjectileQueue)
            {
                StartCoroutine(InitProjectile(pInfo));
                yield return new WaitForSeconds(pInfo.AfterDelay);
            }
            GameManager.Instance.durPattern = false;
        }
    }

    private IEnumerator InitProjectile(LaserInfo pInfo)
    {
        //경고생성
        var warnPos = GetWarningPosition(pInfo.GenPosVector, pInfo.Dir);
        var warningDelayMultiplier = 115f/(GameManager.Instance.Score+25);
        warningDelayMultiplier = Mathf.Clamp(warningDelayMultiplier,0.9f, 2.0f);
        
        GameManager.Instance.RequestWarning(warnPos, pInfo.WarningDelay*warningDelayMultiplier);
        yield return new WaitForSeconds(pInfo.WarningDelay* warningDelayMultiplier);
        //투사체 생성
        var projectile = GetProperProjectile().GetComponent<Laser>();
        projectile.transform.position = GameManager.Instance.GetPositionFromGirdVector(pInfo.GenPosVector);
        projectile.SetDirection(pInfo.Dir);
        projectile.InitProjectile();

        yield return null;
    }

    private GameObject GetProperProjectile()
    {
        return _laserPool.ActivateObject();
    }

    private static IEnumerable<Vector2> GetWarningPosition(Vector2 initPos, Vector2 dir)
    {
        var ret = new List<Vector2>();

        for (var i = 1; i < 6; i += 1)
        {
            ret.Add(dir.x > dir.y
                ? GameManager.Instance.GetPositionFromGirdVector(new Vector2(dir.x * i, initPos.y))
                : GameManager.Instance.GetPositionFromGirdVector(new Vector2(initPos.x, dir.y * i)));
        }

        return ret;
    }

    public void AddPattern(Pattern p)
    {
        _patterns.Enqueue(p);
    }

    public void AddRandomPattern()
    {
        var pList = GameManager.Instance.PatternStore.ListPatterns.Where(a=> a.Difficulty <= GameManager.Instance.Score / 15f).ToList();
        var ran = Random.Range(0, pList.Count);
        
        AddPattern(pList[ran]);
    }
}

