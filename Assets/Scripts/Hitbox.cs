using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f;
    private Collider hitboxCollider;

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
    }

    public void ActivateHitbox()
    {
        hitboxCollider.enabled = true;
        StartCoroutine(DisableHitbox());
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(0.1f);
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            hurtbox.TakeDamage(damage);
        }
    }
}

