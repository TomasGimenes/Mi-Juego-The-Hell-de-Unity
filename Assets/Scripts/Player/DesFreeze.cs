using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesFreeze : MonoBehaviour
{
    Animator anim;
    [SerializeField]private int pulseEMax;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool exitFreeze = false;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Active"))
        {
            pulseEMax = pulseEMax - 1;
            exitFreeze = true;
        }
        else
        {
            exitFreeze = false;
        }
        anim.SetBool("ExitFreeze", exitFreeze);

        if(pulseEMax <= 0)
        {
            GameObject player = Instantiate(playerPrefab);
            player.transform.position = transform.position;
            Destroy(gameObject);
        }

    }
}
