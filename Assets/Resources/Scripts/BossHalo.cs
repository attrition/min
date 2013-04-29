using UnityEngine;
using System.Collections.Generic;

public class BossHalo : MonoBehaviour
{
    private List<GameObject> halo;

    public int HaloElements = 10;

    private float haloRadius = 0.4f;
    private float defaultHaloSpeed = 1.3f;
    private float defaultHaloRadius = 0.4f;
    private float desiredHaloRadius = 0.4f;
    private float largeHaloRadius = 0.9f;
    private float largeHaloSpeed = 1.3f;
    private bool largeHalo = false;
    private float spreadTime = 15f; // lower is faster

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
            position.x = (float)(Mathf.Cos(rad) * defaultHaloRadius + this.transform.position.x);
            position.y = (float)(Mathf.Sin(rad) * defaultHaloRadius + this.transform.position.y);

            var ele = Instantiate(Resources.Load("Prefabs/HaloElement"), position, Quaternion.identity) as GameObject;
            var he = ele.GetComponent<HaloElement>();
            he.Team = this.Team;
            
            ele.renderer.material = this.renderer.material;
            ele.transform.parent = this.transform;

            halo.Add(ele);
        }
    }

    void FixedUpdate()
    {
        largeHalo = false;
        if (Player != null)
        {
            var dist = Vector3.Distance(Player.transform.position, this.transform.position);
            if (dist > 0.8f && dist < 1.5f)
                largeHalo = true;
        }

        if (largeHalo)
            desiredHaloRadius = largeHaloRadius;
        else
            desiredHaloRadius = defaultHaloRadius;

        var tickMovement = desiredHaloRadius / spreadTime;
        if (haloRadius < desiredHaloRadius)
            haloRadius = Mathf.Min(haloRadius + tickMovement, desiredHaloRadius);
        else if (haloRadius > desiredHaloRadius)
            haloRadius = Mathf.Max(haloRadius - tickMovement, desiredHaloRadius);
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

        var mob = collision.gameObject.GetComponent<Mob>();
        if (mob != null && mob.Team == this.Team)
            return;

        var boss = collision.gameObject.GetComponent<Boss>();
        if (boss != null && boss.Team == this.Team)
            return;

        var explosion = Instantiate(Resources.Load("Prefabs/Explosion"), this.transform.position, this.transform.rotation) as GameObject;
        Destroy(explosion, 1f);

        var go = new GameObject("boom!");
        var aud = go.AddComponent<AudioSource>();
        aud.PlayOneShot(Resources.Load("Sounds/boom") as AudioClip);
        Destroy(aud, 2f);
        
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < HaloElements; ++i)
        {
            if (halo[i] == null)
                continue;

            var ele = halo[i];
            var position = this.transform.position;

            var rad = (i / (float)HaloElements) * 2 * Mathf.PI;
            var speed = largeHalo ? largeHaloSpeed : defaultHaloSpeed;

            var clockwise = 1f;

            position.x = (float)(Mathf.Cos(rad - (Time.time * speed) * clockwise) * haloRadius + position.x);
            position.y = (float)(Mathf.Sin(rad - (Time.time * speed) * clockwise) * haloRadius + position.y);

            ele.transform.position = position;
        }
    }
}
