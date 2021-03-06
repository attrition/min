using UnityEngine;
using System.Collections.Generic;

public class Mob : MonoBehaviour
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
    private float moveSpeed = 1.7f;
    private float spreadTime = 15f; // lower is faster

    public Mob Player = null;
    public int Team = 0;
    public bool LargeHaloOverride = false;
    public bool LargeHaloOverrideValue = false;

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

            var ele = Instantiate(Resources.Load("Prefabs/HaloElement"), position, this.transform.rotation) as GameObject;
            var he = ele.GetComponent<HaloElement>();
            he.Team = this.Team;

            ele.renderer.material = this.renderer.material;
            ele.transform.parent = this.transform;

            halo.Add(ele);
        }
    }

    void FixedUpdate()
    {
        if (Player == this)
        {
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
                largeHalo = true;
            else
                largeHalo = false;
        }
        else // ai
        {            
            largeHalo = false;
            if (Player != null)
            {
                var dist = Vector3.Distance(Player.transform.position, this.transform.position);
                if (dist > 0.8f && dist < 1.5f)
                    largeHalo = true;
            }
        }

        if (LargeHaloOverride)
            largeHalo = LargeHaloOverrideValue;

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
        Vector3 pos = Vector3.zero;
        if (Player == this)
        {
            // position is normalized and multiplied by moveSpeed later
            if (Input.GetKey(KeyCode.A))
                pos.x -= 1f;
            if (Input.GetKey(KeyCode.D))
                pos.x += 1f;
            if (Input.GetKey(KeyCode.W))
                pos.y += 1f;
            if (Input.GetKey(KeyCode.S))
                pos.y -= 1f;
        }
        else // ai
        {
            if (Player != null)
            {
                var myPos = this.transform.position;
                var playerPos = Player.transform.position;

                var dist = Vector3.Distance(myPos, playerPos);

                if (dist > 0.9f)
                    pos = playerPos - myPos; // move towards
                else
                    pos = myPos - playerPos; // move away;
            }
        }

        // relative move
        pos.Normalize();
        pos *= (Player == this) ? moveSpeed : moveSpeed * 0.75f;
        pos *= Time.deltaTime;
        this.transform.position += pos;

        pos = this.transform.position;
        pos.x = Mathf.Max(pos.x, -6f);
        pos.y = Mathf.Max(pos.y, -5f);
        pos.x = Mathf.Min(pos.x, 6f);
        pos.y = Mathf.Min(pos.y, 5f);
        this.transform.position = pos;

        for (int i = 0; i < HaloElements; ++i)
        {
            if (halo[i] == null)
                continue;

            var ele = halo[i];
            var position = this.transform.position;

            var rad = (i / (float)HaloElements) * 2 * Mathf.PI;
            var speed = largeHalo ? largeHaloSpeed : defaultHaloSpeed;

            var clockwise = (this == Player ? 1f : -1f);

            position.x = (float)(Mathf.Cos(rad - (Time.time * speed) * clockwise) * haloRadius + position.x);
            position.y = (float)(Mathf.Sin(rad - (Time.time * speed) * clockwise) * haloRadius + position.y);

            ele.transform.position = position;
        }
    }
}
