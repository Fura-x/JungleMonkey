using System.Collections;
using UnityEngine;

public class TramposnakeBehavior : MonoBehaviour
{
    [SerializeField] Transform target = null;
    [SerializeField] Transform mouth = null;
    [SerializeField] [Tooltip("Object MUST be unique.")]GameObject venom = null;

    [SerializeField] float attackCoolDown = 3f;

    bool isAttacking = false;

    IEnumerator CoAttack()
    {
        isAttacking = true;

        while (isAttacking)
        {
            venom.transform.position = mouth.position + new Vector3(0f, -0.5f, 0f);
            venom.SetActive(true);
            venom.transform.right = (target.position - mouth.position).normalized;

            yield return new WaitForSeconds(attackCoolDown);
            isAttacking = false;
        }
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Attack()
    {
        if(!isAttacking) StartCoroutine(CoAttack());
    }
}
