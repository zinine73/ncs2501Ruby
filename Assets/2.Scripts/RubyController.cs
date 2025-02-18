using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public const float FIXED_PT = 0.5f;
    public const int FORCE_PT = 300;
    public float moveSpeed = 4.0f; 
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;
    public ParticleSystem collEffectPrefab;
    public AudioClip throwClip;
    public AudioClip hitClip;

    private bool isInvincible;
    private float invicibleTimer;
    private int currentHealth;
    private Rigidbody2D rb2d;
    private Vector2 position;
    private Animator animator;
    private Vector2 lookDirection = new Vector2(1,0);
    private AudioSource audioSource;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        position = rb2d.position;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // GetAxisLaw를 사용하면 -1,1값이 넘어온다
        //float vertical = Input.GetAxisRaw("Vertical");
        //Debug.Log($"H:{horizontal}");
        //Debug.Log($"V:{vertical}");
        
        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f) ||
            !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        //position.x += moveSpeed * horizontal * Time.deltaTime;
        //position.y += moveSpeed * vertical * Time.deltaTime;
        position += move * moveSpeed * Time.deltaTime;
        rb2d.MovePosition(position);

        if (isInvincible)
        {
            invicibleTimer -= Time.deltaTime;
            if (invicibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                rb2d.position + Vector2.up * 0.2f,
                lookDirection,
                1.5f,
                LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                //Debug.Log("Raycast has hit " + hit.collider.gameObject);
                //NPC jambi = hit.collider.GetComponent<NPC>();
                //if (jambi != null)
                if (hit.collider.TryGetComponent<NPC>(out var jambi))
                {
                    jambi.DisplayDialog();
                }
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invicibleTimer = timeInvincible;
            //Instantiate(collEffectPrefab, transform);
            Instantiate(collEffectPrefab, 
                rb2d.position + Vector2.up * 0.2f, Quaternion.identity);
            PlaySound(hitClip);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        //Debug.Log($"{currentHealth}/{maxHealth}");
        UIHealthBar.instance.SetValue(currentHealth/(float)maxHealth);
    }

    private void Launch()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab, 
            rb2d.position + Vector2.up * FIXED_PT,
            Quaternion.identity);
        Projectile projectile = 
            projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, FORCE_PT);

        animator.SetTrigger("Launch");
        PlaySound(throwClip);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
