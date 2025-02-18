using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public ParticleSystem getEffect;
    public AudioClip collectedClip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        if (controller != null)
        {
            if (controller.health < controller.maxHealth)
            {
                Instantiate(getEffect, transform);
                controller.ChangeHealth(1);
                GetComponent<SpriteRenderer>().enabled = false;
                Destroy(gameObject, 3.0f);

                controller.PlaySound(collectedClip);
            }
        }
    }
}
