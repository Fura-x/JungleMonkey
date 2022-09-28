using System.Collections;
using UnityEngine;

public class GiraffScript : MonoBehaviour
{
    Animation anim = null;

    bool isLow = false;
    [SerializeField] float TimeBetweenSwitch = 10f;

    IEnumerator SwitchPosition()
    {
        yield return new WaitForSeconds(TimeBetweenSwitch);

        if (!isLow)
            anim.Play("LoweringGiraffNeck");
        else
            anim.Play("RaisingGiraffNeck");

        isLow = !isLow;

        StartCoroutine(SwitchPosition());
    }
    private void Start()
    {
        anim = GetComponent<Animation>();
        StartCoroutine(SwitchPosition());
    }
}
