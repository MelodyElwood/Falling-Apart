using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SystemType { OXYGEN_GENERATOR, CO2_SCRUBBER, PRESSURIZER, SOLAR_PANELS, BATTERY_CHARGER, MAIN_LIGHTS, BACKUP_LIGHTS, MAIN_COMPUTER };

public class SystemScript : MonoBehaviour
{
    public SystemType systemType;
    public bool forcedWorking = false;

    public SystemClass system;
    // Start is called before the first frame update
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
            default:
                Debug.LogError("Unkown System Type: " + systemType);
                break;
        }
    }
}

public abstract class SystemClass
{
    public List<ComponentType> requiredComponents = new List<ComponentType>();
    public List<Component> systemComponents = new List<Component>();

    public bool isWorking()
    {
        //First check if it's functional
        foreach (ComponentType ct in requiredComponents)
        {
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

        //Then check if it's connected to power (Change this to also check if power is running)
        if(this.isConnected())
        {
            return true;
        }

        //If not, check that the battery works
        foreach (Component c in systemComponents)
        {
            if (c.type == ComponentType.BATTERY && !c.isBroken)
            {
                Debug.Log("Battery for " + this + " is out of charge.");
                return this.useBattery(); //Returns false if it's not working
            }
        }

        //Finaly send back false if it didn't have a power connector and battery
        Debug.Log(this + " Does not have a battery or power connector that work");
        foreach(Component c in systemComponents) Debug.LogWarning(c + " IsBroken: " + c.isBroken + " Type: " + c.type + " IsAccepted: " + (c.type == ComponentType.POWER_CONNECTOR && !c.isBroken));
        return false;
    } //Check if it is working        (NEEDS TO BE FIXED WITH getComponent)

    public bool isConnected()
    {
        foreach (Component c in systemComponents)
        {
            if (c.type == ComponentType.POWER_CONNECTOR && !c.isBroken)
            {
                return true;
            }
        }
        return false;
    } //Check if it is connected to the power network     (DOES NOT CHECK IF POWER IS ON)

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

    public bool useBattery() //returns false if battery is not charged
    {
        foreach(Component c in systemComponents)
        {
            if(c.type == ComponentType.BATTERY)
            {
                Battery battery = (Battery)c;
                return battery.consumeCharge();
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
        requiredComponents.Add(ComponentType.PUMP);
        requiredComponents.Add(ComponentType.NITROGEN_TANK);
    }
}