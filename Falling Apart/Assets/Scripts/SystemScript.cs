using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SystemType { OXYGEN_GENERATOR, CO2_SCRUBBER, PRESSURIZER, SOLAR_PANELS, BATTERY_CHARGER, MAIN_LIGHTS, BACKUP_LIGHTS, MAIN_COMPUTER, REPAIR_STATION };


public class SystemScript : MonoBehaviour
{
    public static bool powerIsOn;
    public SystemType systemType;
    public bool forcedWorking = false;

    public SystemClass system;

    void Awake()
    {
        switch (systemType)
        {
            case SystemType.OXYGEN_GENERATOR:
                system = new OxygenGenerator();
                break;
            case SystemType.CO2_SCRUBBER:
                system = new Co2Scrubber();
                break;
            case SystemType.PRESSURIZER:
                system = new Pressurizer();
                break;
            case SystemType.SOLAR_PANELS:
                system = new SolarPanels();
                break;
            case SystemType.BACKUP_LIGHTS:
                system = new BackupLights();
                break;
            case SystemType.BATTERY_CHARGER:
                system = new BatteryCharger();
                break;
            case SystemType.MAIN_LIGHTS:
                system = new MainLights();
                break;
            case SystemType.REPAIR_STATION:
                system = new RepairStation();
                break;
            default:
                Debug.LogError("Unkown System Type: ", this);
                break;
        }
    }
}

public abstract class SystemClass
{
    public List<ComponentType> requiredComponents = new List<ComponentType>();
    public List<Component> systemComponents = new List<Component>();

    public virtual void runTick() {}

    public virtual bool isWorking(bool consumeCharge)
    {
        //First check if it's functional
        foreach (ComponentType ct in requiredComponents)
        {
            //Debug.Log(this + " testing " + ct);
            bool hasThisComponent = false;
            foreach (Component c in systemComponents)
            {
                if (c.type == ct)
                {
                    if (!c.isBroken)
                    {
                        hasThisComponent = true;
                        break;
                    }
                }
            }
            if (!hasThisComponent)
            {
                Debug.Log(this + " Does not have " + ct);
                return false;
            }
        }

        //Then check if it's connected to power
        if(this.isConnected())
        {
            return true;
        }

        //If not, check that the battery works
        foreach (Component c in systemComponents)
        {
            if (c.type == ComponentType.BATTERY && !c.isBroken)
            {
                //Debug.Log("Battery for " + this + " is being used.");
                return this.useBattery(consumeCharge); //Returns false if it's not working
            }
        }

        //Finaly send back false if it didn't have a power connector and battery
        Debug.LogWarning(this + " is offline");
        foreach(Component c in systemComponents) Debug.LogWarning(c + " IsBroken: " + c.isBroken + " Type: " + c.type + " IsAccepted: " + (c.type == ComponentType.POWER_CONNECTOR && !c.isBroken));
        return false;
    } //Check if it is working        (NEEDS TO BE FIXED WITH getComponent)

    public bool isConnected()
    {
        foreach (Component c in systemComponents)
        {
            if (c.type == ComponentType.POWER_CONNECTOR && !c.isBroken && SystemScript.powerIsOn)
            {
                return true;
            }
        }
        return false;
    } //Check if it is connected to the power network

    public Component getComponent(ComponentType type) //Finds a component based on a type and returns it, returns null if there is no component.
    {
        foreach (Component c in systemComponents)
        {
            if (c.type == type)
            {
                return c;
            }
        }
        return null;
    }

    public bool hasComponent(ComponentType type) //Checks if it has a component based on a type
    {
        foreach (Component c in systemComponents)
        {
            if (c.type == type)
            {
                return true;
            }
        }
        return false;
    }

    public bool useBattery(bool consumeCharge) //returns false if battery is not charged
    {
        foreach(Component c in systemComponents)
        {
            if(c.type == ComponentType.BATTERY)
            {
                Battery battery = (Battery)c;
                return battery.consumeCharge(consumeCharge);
            }
        }
        return false;
    }

    public void AddComponent(Component component)
    {
        systemComponents.Add(component);
    }
    public void RemoveComponent(Component component)
    {
        systemComponents.Remove(component);
    }

}

public class OxygenGenerator : SystemClass
{
    public OxygenGenerator()
    {
        requiredComponents.Add(ComponentType.FUSE);
        requiredComponents.Add(ComponentType.PUMP);
    }
}

public class Co2Scrubber : SystemClass
{
    public Co2Scrubber()
    {
        requiredComponents.Add(ComponentType.FUSE);
        requiredComponents.Add(ComponentType.PUMP);
        requiredComponents.Add(ComponentType.FILTER);
        requiredComponents.Add(ComponentType.CO2_TANK);
    }
}

public class Pressurizer : SystemClass
{
    public Pressurizer()
    {
        requiredComponents.Add(ComponentType.FUSE);
        requiredComponents.Add(ComponentType.PUMP);
        requiredComponents.Add(ComponentType.NITROGEN_TANK);
    }
}

public class SolarPanels : SystemClass
{
    public SolarPanels()
    {
        requiredComponents.Add(ComponentType.FUSE);
    }

    
    public override void runTick() {
        base.runTick();
        if (this.isWorking(true))
        {
            SystemScript.powerIsOn = true;
        }
    }
}

public class MainLights : SystemClass
{
    GameObject mainlights;
    public MainLights()
    {
        requiredComponents.Add(ComponentType.FUSE);
        requiredComponents.Add(ComponentType.POWER_CONNECTOR);
        mainlights = GameObject.Find("MainLights");
    }
    public override void runTick()
    {
        base.runTick();
        if (this.isWorking(true))
        {
            //Main light empty on
            mainlights.SetActive(true);
        }
        else
        {
            //Main light empty off
            mainlights.SetActive(false);
        }
    }
}

public class BackupLights : SystemClass
{
    GameObject backuplights;
    public BackupLights()
    {
        requiredComponents.Add(ComponentType.POWER_CONNECTOR);
        backuplights = GameObject.Find("BackUpLights");
    }
    public override void runTick()
    {
        base.runTick();
        if (this.isWorking(true))
        {
            //backup light empty on
            backuplights.SetActive(true);
        }
        else
        {
            //backup light empty off
            backuplights.SetActive(false);
        }
    }
}

public class BatteryCharger : SystemClass
{
    public BatteryCharger()
    {
        requiredComponents.Add(ComponentType.FUSE);
        requiredComponents.Add(ComponentType.POWER_CONNECTOR);
    }
    public override void runTick()
    {
        base.runTick();
        if (this.isWorking(true))
        {
            foreach(Component c in systemComponents)
            {
                if(c.type == ComponentType.BATTERY)
                {
                    ((Battery)c).recharge();
                }
            }
        }
    }

    public int[] getBatteryPercentage() //returns the charge percentage of both batteries in the system.
    {
        int battery1Charge = 0;
        int battery2Charge = 0;
        bool firstBattery = true;
        foreach (Component c in systemComponents)
        {
            if (c.type == ComponentType.BATTERY)
            {
                if (firstBattery)
                {
                    battery1Charge = ((Battery)c).getCharge();
                    firstBattery = false;
                }
                else
                {
                    battery2Charge = ((Battery)c).getCharge();
                }
            }
        }
        return new int[] { battery1Charge, battery2Charge };
    }
}

public class RepairStation : SystemClass
{
    public Component componentRepairing;
    public int repairTimeMultiplier = 20;

    public RepairStation()
    {
        requiredComponents.Add(ComponentType.FUSE);
    }

    public override bool isWorking(bool consumeCharge)
    {
        //Check if it's functional (Repair station has an internal reactor, so doesn't need a connection to power)
        foreach (ComponentType ct in requiredComponents)
        {
            if(!hasComponent(ct) || getComponent(ct).isBroken)
            {
                Debug.Log(this + " Does not have " + ct);
                return false;
            }
        }
        return true;
    }

    public override void runTick()
    {
        base.runTick();
        if (componentRepairing != null)
        {
            if (componentRepairing.isBroken)
            {
                componentRepairing.repairTime++;
                if (componentRepairing.repairTime > repairTimeMultiplier * componentRepairing.repairCost)
                {
                    componentRepairing.repairTime = 0;
                    componentRepairing.isBroken = false;
                    Debug.Log("Repair Complete On: " + componentRepairing.type);
                }
            }
        }
    }

    public double getRepairPercentage()
    {
        if (componentRepairing == null)
        {
            return 0;
        }
        else if(!componentRepairing.isBroken)
        {
            return 100;
        }
        else
        {
            return (componentRepairing.repairTime / (repairTimeMultiplier * componentRepairing.repairCost)) * 100;
        }
    }
}