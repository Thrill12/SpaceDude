using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<Commodity> commsInInventory;

    private PrefabManager pf;
    public GameObject inventoryElements;
    public UIManager ui;

    private void Start()
    {
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
    }

    public void AddToInventory(Commodity comm)
    {
        if(commsInInventory.Where(x => x.commodityName == comm.commodityName).Count() != 0)
        {
            commsInInventory.Where(x => x.commodityName == comm.commodityName).First().stack += comm.stack;
        }
        else
        {
            commsInInventory.Add(comm);
            GameObject newUIItem = Instantiate(pf.commodityInventoryDisplayObject, inventoryElements.transform);
            newUIItem.transform.Find("Commodity Icon").GetComponent<Image>().sprite = comm.commodityIcon;
            newUIItem.transform.Find("Commodity Name").GetComponent<TMP_Text>().text = comm.commodityName;
            newUIItem.transform.Find("Commodity Units").GetComponent<TMP_Text>().text = comm.stack + " units";
        }
    }
}
