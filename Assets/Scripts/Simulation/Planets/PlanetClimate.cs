using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetClimate
{
    public TemperatureRange temperatureRange;
    public float humidity;
    public float precipitationPerMonth;
    public float atmosphericPressureASL;

    public PlanetClimate()
    {

    }

    public PlanetClimate(TemperatureRange temperatureRange, float humidity, float precipitationPerMonth, float atmosphericPressureASL)
    {
        this.temperatureRange = temperatureRange;
        this.humidity = humidity;
        this.precipitationPerMonth = precipitationPerMonth;
        this.atmosphericPressureASL = atmosphericPressureASL;
    }

    public bool CanResourceGrow(RawResource resource)
    {
        if(CheckTemperature(resource) && CheckHumidity(resource) && CheckPrecipitation(resource) && CheckPressure(resource))
        {
            return true;
        }

        return false;
    }

    //Checking different variables to see if a resource can grow/be found in this climate
    private bool CheckTemperature(RawResource resource)
    {
        if(resource.climateToGrow.temperatureRange.min != 0 && resource.climateToGrow.temperatureRange.max != 0)
        {
            if ((resource.climateToGrow.temperatureRange.min > temperatureRange.min && resource.climateToGrow.temperatureRange.min <= temperatureRange.max) ||
            (resource.climateToGrow.temperatureRange.max <= temperatureRange.max && resource.climateToGrow.temperatureRange.min <= temperatureRange.max))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private bool CheckHumidity(RawResource resource)
    {
        if (Mathf.Abs(resource.climateToGrow.humidity - humidity) <= resource.climateToGrow.humidityVariation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckPrecipitation(RawResource resource)
    {
        if (Mathf.Abs(resource.climateToGrow.precipitationPerMonth - precipitationPerMonth) <= resource.climateToGrow.precipitationVariation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckPressure(RawResource resource)
    {
        if (Mathf.Abs(resource.climateToGrow.atmosphericPressureASL - atmosphericPressureASL) <= resource.climateToGrow.atmosphericVariation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class ResourceClimateRequirement : PlanetClimate
{
    public float humidityVariation;
    public float precipitationVariation;
    public float atmosphericVariation;
}

[System.Serializable]
public class TemperatureRange
{
    public float min;
    public float max;
}
