using UnityEngine;
using System.Collections;

public class HaloElement : MonoBehaviour
{
    public int Team = 0;

    // Use this for initialization
    void Start()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        var halo = collision.gameObject.GetComponent<HaloElement>();
        if (halo != null && halo.Team == this.Team)
            return;

        var mob = collision.gameObject.GetComponent<Mob>();
        if (mob != null && mob.Team == this.Team)
            return;

        var bossHalo = collision.gameObject.GetComponent<BossHalo>();
        if (bossHalo != null && bossHalo.Team == this.Team)
            return;

        var boss = collision.gameObject.GetComponent<Boss>();
        if (boss != null && boss.Team == this.Team)
            return;

        var explosion = Instantiate(Resources.Load("Prefabs/Explosion"), this.transform.position, this.transform.rotation) as GameObject;
        Destroy(explosion, 1f);

        var go = new GameObject("pop!");
        var aud = go.AddComponent<AudioSource>();
        aud.PlayOneShot(Resources.Load("Sounds/pop") as AudioClip);
        Destroy(aud, 2f);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
