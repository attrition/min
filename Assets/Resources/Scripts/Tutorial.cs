using UnityEngine;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    GameObject startText = null;

    List<GameObject> tutObjects;
    List<Mob> tutMobs = null;

    // Use this for initialization
    void Start()
    {
        tutObjects = new List<GameObject>();
        tutMobs = new List<Mob>();

        var playerTut1 = SpawnTutPlayer(new Vector3(-2.5f, -0.5f, -1f));
        var playerTut2 = SpawnTutPlayer(new Vector3(2.5f, -0.5f, -1f));
        playerTut2.LargeHaloOverride = true;
        playerTut2.LargeHaloOverrideValue = true;

        tutMobs.Add(playerTut1);
        tutMobs.Add(playerTut2);

        tutMobs.Add(SpawnTutMob(new Vector3(1.25f, -3.25f, -1f), 1, 4));
        tutMobs.Add(SpawnTutMob(new Vector3(2.25f, -3.25f, -1f), 2, 7));
        tutMobs.Add(SpawnTutMob(new Vector3(3.25f, -3.25f, -1f), 3, 10));

        SpawnText("minsweeper", new Vector3(0f, 4f, -1f), 0.15f);
        SpawnText("WASD to move", new Vector3(0f, 2.5f, -1f), 0.05f);
        SpawnText("Left Mouse/Spacebar to expand", new Vector3(0f, 2f, -1f), 0.05f);
        startText = SpawnText(">> SPACEBAR TO START << ", new Vector3(0f, 1f, -1f), 0.04f);

        var trickText = SpawnText(">> PRESS ENTER TO SKIP TO BOSS << ", new Vector3(0f, 0.65f, -1f), 0.025f);
        trickText.renderer.material.color = Color.red;

        SpawnText("Normal", new Vector3(-2.5f, -1.7f, -1f), 0.05f);
        SpawnText("Expanded", new Vector3(2.5f, -1.7f, -1f), 0.05f);
        SpawnText("Smack these guys:", new Vector3(-1.5f, -3.25f, -1f), 0.05f);
        SpawnText("Don't get smacked!", new Vector3(0f, -4.5f, -1f), 0.05f);
    }

    Mob SpawnTutPlayer(Vector3 pos, int team = 0, int haloElements = 10)
    {
        var go = Instantiate(Resources.Load("Prefabs/Mob"), pos, this.transform.rotation) as GameObject;
        var mob = go.GetComponent<Mob>();
        mob.Team = team;
        mob.transform.parent = this.transform;
        mob.HaloElements = haloElements;
        mob.renderer.material = Resources.Load("Materials/Player") as Material;
        mob.name = "Player";
        tutObjects.Add(go);
        return mob;
    }

    Mob SpawnTutMob(Vector3 pos, int team = 0, int haloElements = 10)
    {
        var go = Instantiate(Resources.Load("Prefabs/Mob"), pos, this.transform.rotation) as GameObject;
        var mob = go.GetComponent<Mob>();
        mob.Team = team;
        mob.transform.parent = this.transform;
        mob.HaloElements = haloElements;
        mob.renderer.material = Resources.Load("Materials/Enemy" + team) as Material;
        mob.name = "Enemy";
        tutObjects.Add(go);
        return mob;
    }

    GameObject SpawnText(string text, Vector3 pos, float scale = 0.1f)
    {
        var go = Instantiate(Resources.Load("Prefabs/MovingText"), pos, Quaternion.identity) as GameObject;
        go.transform.localScale = Vector3.one * scale;
        var mover = go.GetComponent<MoveOnUpdate>();
        mover.Duration = 9999999f;

        go.GetComponent<TextMesh>().text = text;
        tutObjects.Add(go);
        return go;
    }

    void CleanUp()
    {
        foreach (var mob in tutMobs)
            mob.DestroyHalo();

        foreach (var obj in tutObjects)
            Destroy(obj);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        startText.renderer.material.color = new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Sin(Time.time)));

        if (Input.GetKey(KeyCode.Space))
        {
            CleanUp();
            Instantiate(Resources.Load("Prefabs/Game"));
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            CleanUp();
            var go = Instantiate(Resources.Load("Prefabs/Game")) as GameObject;
            var game = go.GetComponent<Game>();
            game.CurrentLevel = 5;
        }
    }
}
