using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class OxygenArea : MonoBehaviour
{
    public List<SlideyDoor> doorsToVacuum = new List<SlideyDoor>();

    public bool isTight;

    private void Update()
    {
        if(doorsToVacuum.Any(x => x.isOpen) && doorsToVacuum.Count > 0)
        {
            isTight = false;
        }
        else
        {
            isTight = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (isTight && collision.GetComponent<PlayerEntity>())
        {
            collision.GetComponent<PlayerEntity>().useOxygen = false;
        }
        else if(!isTight && collision.GetComponent<PlayerEntity>())
        {
            collision.GetComponent<PlayerEntity>().useOxygen = true;
        }
    }
}
