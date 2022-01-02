using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyIndicatorSpawner : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public float maxDistance;

    private List<GameObject> nearbyObjects = new List<GameObject>();
    private List<GameObject> objsDone = new List<GameObject> ();

    public GameObject playerObj;

    private void Update()
    {
        if (playerObj.activeInHierarchy == false) return;

        nearbyObjects = Physics2D.OverlapCircleAll(playerObj.transform.position, maxDistance).ToList().Select(o => o.gameObject).Where(x => x.CompareTag("Player") || x.CompareTag("Enemy")
         || x.CompareTag("Neutral") || x.CompareTag("Neutral")).ToList();

        foreach (var item in nearbyObjects)
        {
            AddObjectToBeTracked(item);
        }
    }

    private void AddObjectToBeTracked(GameObject objToTrack)
    {
        if (!objsDone.Contains(objToTrack))
        {          
            GameObject arrow = Instantiate(indicatorPrefab, playerObj.transform.Find("IndicatorObjects"));
            arrow.GetComponent<NearbyIndicator>().objToRotateTo = objToTrack;
            arrow.GetComponent<NearbyIndicator>().maxDistance = maxDistance;
            arrow.GetComponent<NearbyIndicator>().playerObj = playerObj;
            arrow.GetComponent<NearbyIndicator>().RotateToObject();
            
            objsDone.Add(objToTrack);
        }
    }
}
