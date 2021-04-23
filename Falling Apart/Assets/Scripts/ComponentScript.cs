using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface BreakableComponent
{
    void damage();
}

public enum ComponentType { FUSE, BATTERY, WARNING_LIGHT, WARNING_BUZZER, ALARM_LIGHT, ALARM_BUZZER, SCREEN, POWER_CONNECTOR };


public class ComponentScript : MonoBehaviour
{    
    public ComponentType componentType;

    public Component component;

    void Awake()
    {
        switch (componentType)
        {
            case ComponentType.FUSE:
                component = new Fuse();
                break;
            case ComponentType.BATTERY:
                component = new Battery();
                break;
            case ComponentType.WARNING_LIGHT:
                //component = new WarningLight();
                break;
            case ComponentType.WARNING_BUZZER:
                //component = new WarningBuzzer();
                break;
            case ComponentType.POWER_CONNECTOR:
                component = new PowerConnector();
                break;
            default:
                Debug.LogError("Unkown Component Type", this);
                break;
        }
    }

}


public abstract class Component : BreakableComponent
{
    public bool isBroken = false;
    public float repairCost; //The "cost" to repair, translates to time in the repair-majig
    public ComponentType type;

    public void damage() //The default damage function is just to break the component.
    {
        this.isBroken = true;
    }
}

public class Fuse : Component, BreakableComponent
{
    public Fuse()
    {
        repairCost = 1;
        type = ComponentType.FUSE;
    }
}

public class Battery : Component, BreakableComponent
{
    int charge = 100;
    public Battery()
    {
        repairCost = 2;
        type = ComponentType.BATTERY;
    }

    public bool consumeCharge() //Returns true if it still has power, false if it doesn't
    {
        if (charge <= 0)
        {
            charge = 0;
            return false;
        }
        else
        {
            charge--;
            return true;
        }
    }
    public int recharge() //Returns number to display on recharge station
    {
        if (charge >= 100)
        {
            charge = 100;
            return charge;
        }
        else
        {
            charge++;
            return charge;
        }

    }
}


public class PowerConnector : Component, BreakableComponent
{
    public PowerConnector()
    {
        repairCost = 2;
        type = ComponentType.POWER_CONNECTOR;
    }

    //Add in checking if it's connected to power
}