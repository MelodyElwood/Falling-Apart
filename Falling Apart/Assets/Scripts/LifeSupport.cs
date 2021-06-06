using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float safeMinAtmospheres = 0.0617f;

    [Space(10)]
    public SystemScript oxygenGeneratorScript;
    public SystemScript CO2ScrubberScript;
    public SystemScript pressurizerScript;
    public SystemScript solarPannelScript;
    public Death overlay;
    public Text deathtext;
    
    [Space(10)]
    public float O2Steps = 1.1f;
    public float CO2Steps = 1.1f;
    public float NDrain = 0.4f;
    [Space(5)]
    public float waitToStart = 0;
    public float tickTime = 1;
    public static int ticksTillDeath = 6;
    int currentTicksTillDeath = ticksTillDeath;

    [Space(10)]
    [Header("Mission Control Variables")]
    public string missionControlManualVersion;
    public GameObject texBoxParent; //the parent for the mission control textboxes.
    public Scrollbar eventLogScrollbar;
    public Text eventLog;
    public int eventLogMaxLength = 10000;


    [Header("Only References")]
    public float currentO2;
    public float currentCO2;
    public float currentN;

    [Space(10)]
    public float currentAtmospheres;
    public float pN;
    public float pO2;
    public float pCO2;

    OxygenGenerator oxygenGenerator;
    Co2Scrubber co2Scrubber;
    Pressurizer pressurizer;
    SolarPanels solarPanels;


    SystemScript[] systemsScripts;
    List<SystemClass> systems = new List<SystemClass>();

    private bool hasAddedError;
    private float pScrollBarValue; //Stores the previous scrollbar value (at the start of a tick)

    // Start is called before the first frame update
    void Start()
    {
        //Create list of all systems
        //solarPanels = (SolarPanels)solarPannelScript.system;
        systemsScripts = Object.FindObjectsOfType<SystemScript>();
        //systems.Add(solarPanels); //Make sure the power system is checked first
        foreach(SystemScript s in systemsScripts)
        {
            //Debug.Log(s);
            if (s != null)
            {
                //Debug.Log(s.system);
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
        pScrollBarValue = eventLogScrollbar.value; //Get where the scroll bar is at the start of the tick

        //Run all of the systems that need to run on tick
        foreach (SystemClass s in systems)
        {
            s.runTick();
            
        }

        //Activate life support systems
        if (oxygenGenerator.isWorking(true) || oxygenGeneratorScript.forcedWorking)
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
        currentN -= NDrain;

        //can't have negative oxygen, co2, or Nitrogen
        if (currentO2 < 0) currentO2 = 0;
        if (currentCO2 < 0) currentCO2 = 0;
        if (currentN < 0) currentN = 0;

        //Check atmosphere
        currentAtmospheres = getCurrentAtmospheres();
        pO2 = getPercentage(currentO2);
        pCO2 = getPercentage(currentCO2);
        pN = getPercentage(currentN);

        //Check if dying
        if (currentAtmospheres > safeMaxAtmospheres || currentAtmospheres < safeMinAtmospheres)
        {
            Debug.Log("Warning, Pressure");
            addToEventLog("Warning, Pressure reaching dangerous levels");
            addToEventLog("\n---------------------\n");
            currentTicksTillDeath--;
            if (currentTicksTillDeath < 0)
            {
                die(currentAtmospheres > safeMaxAtmospheres ? "Nitrogen Narcosis" : "Hypoxia");
            }
        }
        else if(pO2 > safeMaxO2 || pO2 < safeMinO2)
        {
            Debug.Log("Warning, O2");
            addToEventLog("Warning, Oxygen reaching dangerous levels");
            addToEventLog("\n---------------------\n");
            currentTicksTillDeath--;
            if (currentTicksTillDeath < 0)
            {
                die(pO2 > safeMaxO2 ? "Unknown, possible combustion due to high oxygen atmosphere" : "Asphyxiation");
            }
        }
        else if (pCO2 > safeMaxCO2 || pCO2 < safeMinCO2)
        {
            Debug.Log("Warning, CO2");
            addToEventLog("Warning, Carbon Dioxide reaching dangerous levels");
            addToEventLog("\n---------------------\n");
            currentTicksTillDeath--;
            if (currentTicksTillDeath < 0)
            {
                die(pCO2 > safeMaxCO2 ? "Hypercapnia" : "Unknown, death due to extremely low CO2 content");
            }
        }
        else
        {
            currentTicksTillDeath = ticksTillDeath;
        }

        updateMissionControl();
    }

    float getCurrentAtmospheres()
    {
        return (currentO2 + currentCO2 + currentN)/100;
    }

    float getPercentage(float x)
    {
        return x/currentAtmospheres;
    }

    public void die(string causeOfDeath) //Kills the player, and displayes the cause of death to mission control
    {
        overlay.FadeIn();
        deathtext.gameObject.SetActive(true);
        deathtext.text = causeOfDeath;
    }

    //Everything past here has to do with the mission control screen.
    private void addErrorsToEventLog(SystemClass s)
    {
        foreach (Component c in s.systemComponents)
        {
            if (c.isBroken)
            {
                hasAddedError = true;
                eventLog.text = eventLog.text + "ERROR: " + c.type + " at " + s + "\n";
            }
        }
    }

    private void addToEventLog(string s)
    {
        eventLog.text = eventLog.text + s + "\n";
    }

    private Text getText(string name)
    {
        return texBoxParent.transform.Find(name).gameObject.GetComponent<Text>();
    }

    private void updateSystemStatus(SystemClass system, bool status)
    {
        getText(system.ToString()).color = status ? new Color(0, 255, 0, 1) : new Color(255, 0, 0, 1);
        getText(system.ToString()).text = status ? "Online" : "Offline";
    }

    private void updateLifeSupportStatus()
    {
        getText("Pressure").text = System.String.Format("{0:0.000}", currentAtmospheres) + " atm";
        getText("Oxygen").text = System.String.Format("{0:0.00}", pO2) + "%";
        getText("Carbon Dioxide").text = System.String.Format("{0:0.00}", pCO2) + "%";
        getText("Nitrogen").text = System.String.Format("{0:0.00}", pN) + "%";
    }

    private void updateBatteryData(BatteryCharger b)
    {
        int[] batteryPercents = b.getBatteryPercentage();
        getText("BatteryCharge 1").text = batteryPercents[0] + "%";
        getText("BatteryCharge 2").text = batteryPercents[1] + "%";
    }
    private void updateRepairData(RepairStation r)
    {
        getText("RepairPercentage").text = System.String.Format("{0:0.00}", r.getRepairPercentage()) + "%";
        
    }

    public void updateMissionControl()
    {
        //Life support
        updateLifeSupportStatus();

        //Error event log and system status
        hasAddedError = false;
        foreach (SystemClass s in systems)
        {
            updateSystemStatus(s, s.isWorking(false));
            addErrorsToEventLog(s);
            
            //Battery Charge and Repair amount update when that system is found
            if (s.ToString() == "BatteryCharger")
            {
                updateBatteryData((BatteryCharger)s);
            }
            else if(s.ToString() == "RepairStation")
            {
                updateRepairData((RepairStation)s);
            }
        }
        if (hasAddedError) addToEventLog("\n---------------------\n"); //Add end bit for seperation if error is found

        //Remove the first part of the event log when it starts to get too long in order to fix issues
        if(eventLog.text.Length > eventLogMaxLength)
        {
            eventLog.text = eventLog.text.Remove(0, eventLog.text.Length - eventLogMaxLength);
        }

        //If scrollbar was at the bottom, move it to the bottom
        if(pScrollBarValue <= 0.1)
        {
            eventLogScrollbar.value = 0;
        }
    }
}
