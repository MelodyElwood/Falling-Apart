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

    private void Start()
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
            default:
                Debug.LogError("Unkown Component Type", this);
                break;
        }
    }

}


public abstract class Component : BreakableComponent
{
    public bool isBroken = false;
    public float repairCost;

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
    }
}

public class Battery : Component, BreakableComponent
{
    int charge = 100;
    public Battery()
    {
        repairCost = 2;
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
