using UnityEngine;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    private List<GameObject> halo;

    public int HaloElements = 4;
    public int ChildHaloElements = 10;

    private float haloRadius = 1.1f;
    private float haloSpeed = 1.3f;
    private float moveSpeed = 1f;

    public Mob Player = null;
    public int Team = 1;

    // Use this for initialization
    void Start()
    {
        halo = new List<GameObject>();
        for (int i = 0; i < HaloElements; ++i)
        {
            var position = this.transform.position;
            var rad = (i / (float)HaloElements) * 2 * Mathf.PI;
            position.x = (float)(Mathf.Cos(rad) * haloRadius + this.transform.position.x);
            position.y = (float)(Mathf.Sin(rad) * haloRadius + this.transform.position.y);

            var ele = Instantiate(Resources.Load("Prefabs/BossHalo"), position, this.transform.rotation) as GameObject;
            var he = ele.GetComponent<BossHalo>();
            he.Team = this.Team;
            he.Player = Player;
            he.HaloElements = ChildHaloElements;

            ele.renderer.material = this.renderer.material;
            ele.transform.parent = this.transform;

            halo.Add(ele);
        }
    }

    public void DestroyHalo()
    {
        foreach (var ele in halo)
            if (ele != null)
                DestroyImmediate(ele.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        var halo = collision.gameObject.GetComponent<HaloElement>();
        if (halo != null && halo.Team == this.Team)
            return;

        var mob = collision.gameObject.GetComponent<BossHalo>();
        if (mob != null && mob.Team == this.Team)
            return;

        var explosion = Instantiate(Resources.Load("Prefabs/SuperExplosion"), this.transform.position, this.transform.rotation) as GameObject;
        Destroy(explosion, 5f);

        var go = new GameObject("boom!");
        var aud = go.AddComponent<AudioSource>();
        aud.PlayOneShot(Resources.Load("Sounds/superexplosion") as AudioClip);
        Destroy(aud, 8f);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Vector3.zero;
        if (Player != null)
        {
            var myPos = this.transform.position;
            var playerPos = Player.transform.position;

            var dist = Vector3.Distance(myPos, playerPos);

            if (dist > 1.7f)
                pos = playerPos - myPos; // move towards
            else
                pos = myPos - playerPos; // move away;
        }

        // relative move
        pos.Normalize();
        pos *= moveSpeed;
        pos *= Time.deltaTime;
        this.transform.position += pos;

        for (int i = 0; i < HaloElements; ++i)
        {
            if (halo[i] == null)
                continue;

            var ele = halo[i];
            var position = this.transform.position;

            var rad = (i / (float)HaloElements) * 2 * Mathf.PI;

            var clockwise = -1f;

            position.x = (float)(Mathf.Cos(rad - (Time.time * haloSpeed) * clockwise) * haloRadius + this.transform.position.x);
            position.y = (float)(Mathf.Sin(rad - (Time.time * haloSpeed) * clockwise) * haloRadius + this.transform.position.y);

            ele.transform.position = position;
        }
    }
}
