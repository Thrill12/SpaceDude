using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void GenerateLightning(GameObject target)
    {
        line = GetComponent<LineRenderer>();

        int numOfPoints = Random.Range(10, 20);

        Vector3 direction = target.transform.position - transform.position;
        Debug.DrawLine(target.transform.position, transform.position, Color.red);
        line.positionCount = numOfPoints;

        line.SetPosition(0, transform.position);

        for (int i = 1; i < numOfPoints - 2; i++)
        {
            line.SetPosition(i, new Vector3((direction.normalized * i/numOfPoints).x + Random.Range(-0.3f, 0.3f), (direction.normalized * i / numOfPoints).y + i + Random.Range(0.2f, 0.5f), 0));
            //Vector3 newPos = line.GetPosition(i);
            //newPos = direction * 1/i + (Vector3)Random.insideUnitCircle * 0.5f;
            //line.SetPosition(i, newPos);
        }

        line.SetPosition(numOfPoints - 1, target.transform.position);

        Debug.Log("Generated");

        Destroy(gameObject, 0.1f);
    }
}
