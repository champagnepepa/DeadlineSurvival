using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerAttack pa;
    private Animator anim;
    public float cooldownTime = 1f;
    private float nextFireTime = 1f;
    public static int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 0.5f;
    public bool IsAttacking = false;

    public List<CollisionDetection> activeWeaponCollisions = new List<CollisionDetection>();

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            anim.SetBool("hit1", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
        {
            anim.SetBool("hit2", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f && anim.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
        {
            anim.SetBool("hit3", false);
            noOfClicks = 0;
        }


        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        //cooldown time
        if (Time.time > nextFireTime)
        {
            // Check for mouse input
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();

            }
        }
    }

    void OnClick()
    {
        lastClickedTime = Time.time;
        noOfClicks++;

        if (noOfClicks == 1)
        {
            anim.SetBool("hit1", true);
            IsAttacking = true;
            PlayAllSwingSounds();
        }

        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);

        var state = anim.GetCurrentAnimatorStateInfo(0);

        if (noOfClicks >= 2 && state.normalizedTime > 0.8f && state.IsName("hit1"))
        {
            anim.SetBool("hit1", false);
            anim.SetBool("hit2", true);
            PlayAllSwingSounds();
        }

        if (noOfClicks >= 3 && state.normalizedTime > 0.8f && state.IsName("hit2"))
        {
            anim.SetBool("hit2", false);
            anim.SetBool("hit3", true);
            IsAttacking = false;
            PlayAllSwingSounds();
        }
    }

    private void PlayAllSwingSounds()
    {
        foreach (var cd in activeWeaponCollisions)
        {
            if (cd != null && cd.gameObject.activeInHierarchy)
            {
                cd.PlaySwingSound();
            }
        }
    }

    public void UpdateWeaponCollisions(Transform handParent)
    {
        activeWeaponCollisions.Clear();

        foreach (Transform child in handParent)
        {
            if (child.gameObject.activeSelf)
            {
                CollisionDetection cd = child.GetComponentInChildren<CollisionDetection>();
                if (cd != null)
                    activeWeaponCollisions.Add(cd);
            }
        }
    }
}