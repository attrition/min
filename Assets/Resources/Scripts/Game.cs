using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    Mob player = null;

    // Use this for initialization
    void Start()
    {
        var go = Instantiate(Resources.Load("Prefabs/Mob"), this.transform.position, this.transform.rotation) as GameObject;
        player = go.GetComponent<Mob>();
        player.transform.parent = this.transform;
        player.name = "Player";
        player.Player = player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
