using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSkullExplosition : MonoBehaviour
{
    [SerializeField]
    List<Rigidbody2D> _rbs;

    [SerializeField]
    List<Dissolver> _dissolvers;

    [SerializeField]
    Vector2 _force;

    [SerializeField]
    float _torque;

    public void Explode()
    {
        foreach (var rb in _rbs)
        {
            var torque = Random.Range(-_torque, _torque);
            rb.AddForce(new Vector2(Random.Range(-_force.x, _force.x), Random.Range(_force.y * 0.7f, _force.y)), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Impulse);
        }

        foreach (var dissolver in _dissolvers)
            dissolver.Dissolve();

        StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
