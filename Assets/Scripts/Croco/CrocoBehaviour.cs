using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocoBehaviour : MonoBehaviour
{
    bool doesBite = false;
    bool mouthClosed = true;
    [SerializeField] float timeBetweenAnimations = 3f;
    Animation anim;

    IEnumerator CrocoAnimationManager()
    {
        yield return new WaitForSeconds(timeBetweenAnimations);
        if (mouthClosed)
            anim.Play("MouthOpening");
        else 
            anim.Play("MouthClosing");

    }
    public void AlertObservers(string message)
    {
        switch(message)
        {
            case "BiteOn":
                doesBite = true;
                break;
            case "BiteOff":
                doesBite = false;
                break;
            case "Mouth closed":
                mouthClosed = true;
                StartCoroutine(CrocoAnimationManager());
                break;
            case "Mouth opened":
                mouthClosed = false;
                StartCoroutine(CrocoAnimationManager());
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        anim = GetComponent<Animation>();
        StartCoroutine(CrocoAnimationManager());
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && doesBite)
        {
            collision.gameObject.GetComponent<Move>().Knockback(new Vector3(0f, 2f, 0f));
            collision.gameObject.GetComponent<Life>().Hurt(1);
        }
    }
}
