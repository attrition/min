using UnityEngine;
using System.Collections;

public class MoveOnUpdate : MonoBehaviour
{
    public Vector3 Direction;
    public float Speed;
    public float Duration;

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, Duration);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += Direction * Speed;
    }
}
