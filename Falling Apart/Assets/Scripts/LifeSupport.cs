using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSupport : MonoBehaviour
{
    //These are in percents
    [Header("To Edit")]
    public float safeMaxCO2 = 15;
    public float safeMinCO2 = 0;
    [Space(5)]
    public float safeMaxO2 = 100; //combustion issues
    public float safeMinO2 = 19; //https://www.skuld.com/contentassets/4867cb8cc9f842a08c25d29050654527/oxygen-levels---skuld.pdf
    [Space(5)]
    public float safeMaxAtmospheres = 6; 
    public float safeMinAtmospheres = 0.07f;

    [Space(10)]
    public SystemScript oxygenGeneratorScript;
    public SystemScript CO2ScrubberScript;
    public SystemScript pressurizerScript;
    public SystemScript solarPannelScript;
    public Death overlay;
    [Space(10)]
    public float O2Steps = 1.2f;
    public float CO2Steps = 1.2f;
    [Space(5)]
    public float waitToStart = 0;
    public float tickTime = 1;

    [Header("Only References")]
    public float currentO2;
    public float currentCO2;
    public float currentN;

    [Space(10)]
    public float pO2;
    public float pCO2;
    public float currentAtmospheres;

    OxygenGenerator oxygenGenerator;
    Co2Scrubber co2Scrubber;
    Pressurizer pressurizer;
    SolarPanels solarPanels;


    SystemScript[] systemsScripts;
    List<SystemClass> systems = new List<SystemClass>();

    // Start is called before the first frame update
    void Start()
    {
        //Create list of all systems
        solarPanels = (SolarPanels)solarPannelScript.system;
        systemsScripts = Object.FindObjectsOfType<SystemScript>();
        systems.Add(solarPanels); //Make sure the power system is checked first
        foreach(SystemScript s in systemsScripts)
        {
            if (s != null)
            {
                systems.Add(s.system);
            }
        }
        

        currentO2 = 21;
        currentCO2 = 1;
        currentN = 78;
        oxygenGenerator = (OxygenGenerator)oxygenGeneratorScript.system;
        co2Scrubber = (Co2Scrubber)CO2ScrubberScript.system;
        pressurizer = (Pressurizer)pressurizerScript.system;

        InvokeRepeating("Run", waitToStart, tickTime);
    }

    void Run()
    {
        //Run all of the systems that need to run on tick
        foreach(SystemClass s in systems)
        {
            s.runTick();
        }
        
        //Activate life support systems
        if(oxygenGenerator.isWorking(true) || oxygenGeneratorScript.forcedWorking)
        {
            //Create O2
            currentO2 += O2Steps;
        }
        if (co2Scrubber.isWorking(true) || CO2ScrubberScript.forcedWorking)
        {
            //Get rid of CO2
            currentCO2 -= CO2Steps;
            //make the filter get more grime
            foreach(Component c in co2Scrubber.systemComponents)
            {
                if (c.type == ComponentType.FILTER)
                {
                    Filter f = (Filter)c;
                    f.filterAir();
                }
            }
        }
        if (pressurizer.isWorking(true) || pressurizerScript.forcedWorking)
        {
            //Stabalize the atmosphere
            if (currentAtmospheres > 1 || (pO2 <= safeMinO2 && currentO2 != 0))
            {
                currentN--;
            }
            else if (currentAtmospheres < 1)
            {
                currentN++;
            }
        }

        //Breathing
        currentO2--;
        currentCO2++;

        //can't have - oxygen, co2, or Nitrogen
        if (currentO2 < 0) currentO2 = 0;
        if (currentCO2 < 0) currentCO2 = 0;
        if (currentN < 0) currentN = 0;

        //Check atmosphere
        currentAtmospheres = getCurrentAtmospheres();
        pO2 = getPercentage(currentO2);
        pCO2 = getPercentage(currentCO2);
        
        if(currentAtmospheres > safeMaxAtmospheres || currentAtmospheres < safeMinAtmospheres)
        {
            Debug.Log("Warning, Preasure");
            die();
        }

        if(pO2 > safeMaxO2 || pO2 < safeMinO2)
        {
            Debug.Log("Warning, O2");
            die();
        }

        if (pCO2 > safeMaxCO2 || pCO2 < safeMinCO2)
        {
            Debug.Log("Warning, CO2");
            die();
        }
    }

    float getCurrentAtmospheres()
    {
        return (currentO2 + currentCO2 + currentN)/100;
    }

    float getPercentage(float x)
    {
        return x/currentAtmospheres;
    }

    public void die()
    {
        overlay.FadeOut();
    }
}
