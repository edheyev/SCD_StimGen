using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void OnDrawGizmosSelected()
    {
        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, radius);
    }
}