using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobraBehaviour : MonoBehaviour
{
    [SerializeField] Transform target = null;
    [SerializeField] Transform mouth = null;
    [SerializeField] GameObject venom = null;
    Animation anim = null;
    Move playerMove = null;

    [SerializeField] float range = 10f;
    [SerializeField] float minRange = 2.5f;
    [SerializeField] float attackCoolDown = 3f;
    [SerializeField] float stunTime = 3f;
    [SerializeField] GameObject stunAnim = null;

    [SerializeField] bool canTurn = true;
    bool isAttacking = false;
    bool isStunned = false;



    IEnumerator Attack()
    {
        while(isAttacking)
        {
            venom.transform.position = mouth.position;
            venom.SetActive(true);
            venom.transform.right = (target.position - mouth.position).normalized;
            yield return new WaitForSeconds(attackCoolDown);
        }
    }

    IEnumerator Stunned()
    {
        isStunned = true;
        stunAnim.SetActive(true);
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        stunAnim.SetActive(false);
    }
    private void Awake()
    {
        anim = GetComponent<Animation>();
        venom.SetActive(false);
        playerMove = FindObjectOfType<Move>();
    }

    private void LateUpdate()
    {
        if (playerMove.isInDepth && isAttacking)
        {
            isAttacking = false;
            anim.Play("CalmedDown");
            StopAllCoroutines();
        }

        if (!anim.isPlaying && !playerMove.isInDepth)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance <= range && !isAttacking && !isStunned)
            {
                isAttacking = true;
                anim.Play("StandUp");
            }
            else if (isStunned && isAttacking)
            {
                isAttacking = false;
                anim.Play("CalmedDown");
            }
            else if(distance > range && isAttacking)
            {
                isAttacking = false;
                StopAllCoroutines();
                anim.Play("CalmedDown");
            }
        }
        if (!isStunned && canTurn && 
            Vector3.Distance(Vector3.right * transform.position.x, Vector3.right * target.position.x) > minRange)
        {
            if (target.position.x > transform.position.x && isAttacking)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void AlertObservers(string message)
    {
        if(message == "Attacking")
        {
            StartCoroutine(Attack());
        }
    }
    public void Stun()
    {
        if (!isStunned)
        {
            StopAllCoroutines();
            StartCoroutine(Stunned());
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
    public bool IsStunned()
    {
        return isStunned;
    }
}
