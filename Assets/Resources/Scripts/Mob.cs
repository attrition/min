using UnityEngine;
using System.Collections.Generic;

public class Mob : MonoBehaviour
{
    private List<GameObject> halo;

    private int haloElements = 10;
    private float haloRadius = 0.4f;
    private float defaultHaloSpeed = 1f;
    private float defaultHaloRadius = 0.4f;
    private float desiredHaloRadius = 0.4f;
    private float largeHaloRadius = 0.8f;
    private float largeHaloSpeed = 2.5f;
    private bool largeHalo = false;
    private float moveSpeed = 1.5f;

    public GameObject Player = null;

    // Use this for initialization
    void Start()
    {
        halo = new List<GameObject>();
        for (int i = 0; i < haloElements; ++i)
        {
            var ele = Instantiate(Resources.Load("Prefabs/HaloElement")) as GameObject;
            ele.transform.parent = this.transform;
            var position = ele.transform.position;

            var rad = (i / (float)haloElements) * 2 * Mathf.PI;
            position.x = (float)(Mathf.Cos(rad) * defaultHaloRadius + this.transform.position.x);
            position.y = (float)(Mathf.Sin(rad) * defaultHaloRadius + this.transform.position.y);
            ele.transform.position = position;

            halo.Add(ele);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            largeHalo = true;
            desiredHaloRadius = largeHaloRadius;
        }
        else
        {
            largeHalo = false;
            desiredHaloRadius = defaultHaloRadius;
        }

        var tickMovement = desiredHaloRadius / 15f;
        if (haloRadius < desiredHaloRadius)
            haloRadius = Mathf.Min(haloRadius + tickMovement, desiredHaloRadius);
        else if (haloRadius > desiredHaloRadius)
            haloRadius = Mathf.Max(haloRadius - tickMovement, desiredHaloRadius);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Vector3.zero;
        if (Player == this.gameObject)
        {
            if (Input.GetKey(KeyCode.A))
                pos.x -= moveSpeed;
            if (Input.GetKey(KeyCode.D))
                pos.x += moveSpeed;
            if (Input.GetKey(KeyCode.W))
                pos.y += moveSpeed;
            if (Input.GetKey(KeyCode.S))
                pos.y -= moveSpeed;
        }
        else // ai
        {

        }

        // relative move
        pos *= Time.deltaTime;
        this.transform.position += pos;

        for (int i = 0; i < haloElements; ++i)
        {
            if (halo[i] == null)
                continue;

            var ele = halo[i];
            var position = this.transform.position;

            var rad = (i / (float)haloElements) * 2 * Mathf.PI;
            var speed = largeHalo ? largeHaloSpeed : defaultHaloSpeed;

            position.x = (float)(Mathf.Cos(rad - (Time.time * speed)) * haloRadius + this.transform.position.x);
            position.y = (float)(Mathf.Sin(rad - (Time.time * speed)) * haloRadius + this.transform.position.y);

            ele.transform.position = position;
        }
    }
}
