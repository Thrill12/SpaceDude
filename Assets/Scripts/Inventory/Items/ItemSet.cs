using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Item Set")]
public class ItemSet : ScriptableObject
{
    public string itemSetName;
    public Color itemSetColour;

    public List<Modifier> modifiersTwoEquipped;
    public List<Modifier> modifiersThreeEquipped;
    public List<Modifier> modifiersFourEquipped;
}
