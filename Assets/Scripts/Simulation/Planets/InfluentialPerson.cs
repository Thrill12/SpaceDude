using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InfluentialPerson : ScriptableObject
{
    public string personName;
    public Planet planetOfOrigin;
    public int age;
    public int influence;
    //-3 to 3 or however many goals i have written down in the plan.txt in diplomacy folder
    public int personality;
    public List<Relationship> relationships;
}

public class Relationship
{
    public InfluentialPerson person;
    public string relation;
}
