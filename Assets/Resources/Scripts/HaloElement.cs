using UnityEngine;
using System.Collections;

public class HaloElement : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("halo start");
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        Debug.Log("destroying");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
