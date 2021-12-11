using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalacticEvent
{
    public string eventDescription;
    public Planet eventSource;


    public GalacticEvent(string eventDescription, Planet eventSource)
    {
        this.eventDescription = eventDescription;
        this.eventSource = eventSource;
    }
}
