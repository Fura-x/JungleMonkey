using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeDisplay : MonoBehaviour
{
    public GameObject[] lifeObjects;

    Life playerLife = null;

    // Start is called before the first frame update
    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();

        playerLife.OnHurt.AddListener(() => UpdateLife());
        playerLife.OnHeal.AddListener(() => UpdateLife());
    }

    // Update is called once per frame
    void UpdateLife()
    {
        for(int i = 0; i < lifeObjects.Length; ++i)
        {
            if (playerLife.life <= i) lifeObjects[i].SetActive(false);
            else lifeObjects[i].SetActive(true);
        }
    }
}
