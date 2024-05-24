using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGenerator : MonoBehaviour
{
    Transform monsterPool = null;

    public GameObject[] EliteMonPrefs = null;
    public GameObject[] BossMonPrefs = null;

    float spawnTime = 0.0f;

    int monLimit = 20;
    int curStage = 0;

    void Start()
    {
        spawnTime = Random.Range(0.1f, 0.3f);

        monsterPool = GameObject.Find("MonsterPool").GetComponent<Transform>();

        curStage = AllSceneMgr.Instance.CurStageNum;
    }

    void Update()
    {
        if (!GameMgr.Inst.hasRing) SpawnNormalMon();
    }

    void SpawnNormalMon()
    {
        spawnTime -= Time.deltaTime;

        if (spawnTime <= 0.0f && MemoryPoolMgr.Inst.ActiveMonsterCount < monLimit)
        {
            spawnTime = Random.Range(0.1f, 0.3f);
            MonsterCtrl monCtrl = MemoryPoolMgr.Inst.AddMonsterPool(curStage);
            monCtrl.gameObject.SetActive(true);
            monCtrl.transform.position = GameMgr.Inst.player.transform.position + GetMonSpawnPos();
        }
    }

    Vector3 GetMonSpawnPos()
    {
        Vector3 pos = Vector3.zero;
        float radius = Random.Range(5.0f, 7.0f);
        float sinDeg = Random.Range(0f, 360f);
        float cosDeg = Random.Range(0f, 360f);

        pos.x = Mathf.Cos(cosDeg * Mathf.Deg2Rad);
        pos.y = Mathf.Sin(sinDeg * Mathf.Deg2Rad);
        pos *= radius;

        return pos;
    }

    public void SpawnEliteMon()
    {
        GameObject eliteMon = Instantiate(EliteMonPrefs[curStage], monsterPool);
        eliteMon.transform.position = GameMgr.Inst.player.transform.position + GetMonSpawnPos();
    }

    public void SpawnBossMon(Vector2 spawnPos)
    {
        //Boss Mon Spawn 시키기
        GameObject bossMon = Instantiate(BossMonPrefs[curStage], monsterPool);
        bossMon.transform.position = spawnPos;
    }
}