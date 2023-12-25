using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_AttackCollider : MonoBehaviour
{
    private Monster monster;

    private void Start()
    {
        monster = GetComponentInParent<Monster>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            monster.StartAttack();
            monster.Chasing(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            monster.Chasing(true);
        }
    }
}
