using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    Mob player = null;
    List<Mob> enemies = null;
    Boss boss = null;

    public int CurrentLevel = 0;
    
    // Use this for initialization
    void Start()
    {
        StartLevel(CurrentLevel);
        CreateBoundary();
    }

    void CreateBoundary()
    {
        var mat = Resources.Load("Materials/Arena") as Material;

        var go1 = new GameObject("Arena Bounds 1");
        go1.transform.parent = this.transform;
        var lr1 = go1.AddComponent<LineRenderer>();
        lr1.material = mat;
        lr1.SetVertexCount(2);
        lr1.SetPosition(0, new Vector3(-6f, -5f, -1f));
        lr1.SetPosition(1, new Vector3(6f, -5f, -1f));
        lr1.SetWidth(0.05f, 0.05f);

        var go2 = new GameObject("Arena Bounds 2");
        go2.transform.parent = this.transform;
        var lr2 = go2.AddComponent<LineRenderer>();
        lr2.material = mat;
        lr2.SetVertexCount(2);
        lr2.SetPosition(0, new Vector3(6f, -5f, -1f));
        lr2.SetPosition(1, new Vector3(6f, 5f, -1f));
        lr2.SetWidth(0.05f, 0.05f);

        var go3 = new GameObject("Arena Bounds 3");
        go3.transform.parent = this.transform;
        var lr3 = go3.AddComponent<LineRenderer>();
        lr3.material = mat;
        lr3.SetVertexCount(2);
        lr3.SetPosition(0, new Vector3(6f, 5f, -1f));
        lr3.SetPosition(1, new Vector3(-6f, 5f, -1f));
        lr3.SetWidth(0.05f, 0.05f);

        var go4 = new GameObject("Arena Bounds 4");
        go4.transform.parent = this.transform;
        var lr4 = go4.AddComponent<LineRenderer>();
        lr4.material = mat;
        lr4.SetVertexCount(2);
        lr4.SetPosition(0, new Vector3(-6f, 5f, -1f));
        lr4.SetPosition(1, new Vector3(-6f, -5f, -1f));
        lr4.SetWidth(0.05f, 0.05f);
    }

    void StartLevel(int level)
    {
        CurrentLevel = level;
        if (CurrentLevel > 5)
            return;

        if (boss != null)
        {
            boss.DestroyHalo();
            DestroyImmediate(boss.gameObject);
        }

        if (enemies != null)
        {
            foreach (var mob in enemies)
            {
                if (mob != null)
                {
                    mob.DestroyHalo();
                    DestroyImmediate(mob.gameObject);
                }
            }
        }
        enemies = new List<Mob>();

        if (player != null)
        {
            player.DestroyHalo();
            DestroyImmediate(player.gameObject);
        }
        SpawnMob(new Vector3(-2.5f, -2.5f, -1f), true);

        switch (level)
        {
            case 0:
                SpawnMob(new Vector3(2.5f, 2.5f, -1f), false, 1, 4);
                break;

            case 1:
                SpawnMob(new Vector3(-2.5f, 2.5f, -1f), false, 1, 4);
                SpawnMob(new Vector3(2.5f, -2.5f, -1f), false, 2, 8);
                break;

            case 2:
                SpawnMob(new Vector3(-2.5f, 2.5f, -1f), false, 1, 4);
                SpawnMob(new Vector3(2.5f, 2.5f, -1f), false, 2, 7);
                SpawnMob(new Vector3(2.5f, -2.5f, -1f), false, 3, 10);
                break;

            case 3:
                SpawnMob(new Vector3(-2.5f, 2.5f, -1f), false, 2, 7);
                SpawnMob(new Vector3(2.5f, 2.5f, -1f), false, 3, 10);
                SpawnMob(new Vector3(2.5f, -2.5f, -1f), false, 2, 7);
                break;

            case 4:
                SpawnMob(new Vector3(-2.5f, 2.5f, -1f), false, 3, 10);
                SpawnMob(new Vector3(2.5f, 2.5f, -1f), false, 3, 10);
                SpawnMob(new Vector3(2.5f, -2.5f, -1f), false, 3, 10);
                break;

            case 5:
                // boss level
                SpawnBoss(new Vector3(2.5f, 2.5f, -1f), 1);
                break;
        }

        SpawnText("Round " + (CurrentLevel + 1), new Vector3(0f, 3.5f, -1f), Vector3.up);
    }

    Boss SpawnBoss(Vector3 pos, int team = 1, int haloElements = 6, int childHaloElements = 8)
    {
        var go = Instantiate(Resources.Load("Prefabs/Boss"), pos, Quaternion.identity) as GameObject;
        boss = go.GetComponent<Boss>();
        boss.Team = team;
        boss.transform.parent = this.transform;
        boss.HaloElements = haloElements;
        boss.ChildHaloElements = childHaloElements;
        boss.Player = player;

        return boss;
    }

    Mob SpawnMob(Vector3 pos, bool isPlayer, int team = 0, int haloElements = 10)
    {
        var go = Instantiate(Resources.Load("Prefabs/Mob"), pos, this.transform.rotation) as GameObject;
        var mob = go.GetComponent<Mob>();
        mob.Team = team;
        mob.transform.parent = this.transform;
        mob.HaloElements = haloElements;

        if (isPlayer)
        {
            mob.renderer.material = Resources.Load("Materials/Player") as Material;
            mob.name = "Player";
            mob.Player = mob;
            player = mob;
        }
        else
        {
            mob.renderer.material = Resources.Load("Materials/Enemy" + team) as Material;
            mob.name = "Enemy";
            mob.Player = player;
            enemies.Add(mob);
        }

        return mob;
    }

    void SpawnText(string text, Vector3 pos, Vector3 dir, float dur = 5f)
    {
        var go = Instantiate(Resources.Load("Prefabs/MovingText"), pos, Quaternion.identity) as GameObject;
        var mover = go.GetComponent<MoveOnUpdate>();
        mover.Direction = dir;
        mover.Duration = dur;

        go.GetComponent<TextMesh>().text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentLevel > 5)
            return;

        if (player == null)
        {
            // wait for keypress, restart level
            SpawnText("Try again!", new Vector3(0f, -3.5f, -1f), Vector3.down);
            StartLevel(CurrentLevel);
        }
        else
        {
            int count = 0;
            
            if (boss != null)
                count++;

            foreach (var mob in enemies)
            {
                if (mob != null)
                    count++;
            }

            if (count == 0)
            {
                // wait for keypress, move to next level
                StartLevel(CurrentLevel + 1);
                
                if (CurrentLevel <= 5)
                    SpawnText("Good job!", new Vector3(0f, -3.5f, -1f), Vector3.down);
            }
        }

        if (CurrentLevel > 5)
        {
            player.Player = null;
            SpawnText("Game Over", new Vector3(0f, 1.5f, -1f), Vector3.zero, 99999f);
            SpawnText("YOU WIN", new Vector3(0f, -1.5f, -1f), Vector3.zero, 99999f);
        }
    }
}
