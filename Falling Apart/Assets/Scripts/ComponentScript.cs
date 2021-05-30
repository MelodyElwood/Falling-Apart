using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface BreakableComponent
{
    void damage();
}

public enum ComponentType { FUSE, BATTERY, WARNING_LIGHT, WARNING_BUZZER, ALARM_LIGHT, ALARM_BUZZER, SCREEN, POWER_CONNECTOR, PUMP , CO2_TANK, NITROGEN_TANK, FILTER};


public class ComponentScript : MonoBehaviour
{    
    public ComponentType componentType;

    public Component component;

    void Awake()
    {
        this.createComponent();
    }

    public void createComponent()
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
                component = new WarningLight();
                break;
            case ComponentType.WARNING_BUZZER:
                component = new WarningBuzzer();
                break;
            case ComponentType.ALARM_LIGHT:
                component = new AlarmLight();
                break;
            case ComponentType.ALARM_BUZZER:
                component = new AlarmBuzzer();
                break;
            case ComponentType.SCREEN:
                component = new ScreenComponent();
                break;
            case ComponentType.POWER_CONNECTOR:
                component = new PowerConnector();
                break;
            case ComponentType.PUMP:
                component = new Pump();
                break;
            case ComponentType.CO2_TANK:
                component = new CO2Tank();
                break;
            case ComponentType.NITROGEN_TANK:
                component = new NitrogenTank();
                break;
            case ComponentType.FILTER:
                component = new Filter();
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
    public int repairTime; //How long it has been repairing
    public ComponentType type;

    public virtual void damage() //The default damage function is just to break the component.
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

    public bool consumeCharge(bool consumeCharge) //Returns true if it still has power, false if it doesn't
    {
        if (charge <= 0)
        {
            charge = 0;
            return false;
        }
        else
        {
            if(consumeCharge) charge--;
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

public class WarningLight : Component, BreakableComponent
{
    public WarningLight()
    {
        repairCost = 0.5f;
        type = ComponentType.WARNING_LIGHT;
    }
}
public class WarningBuzzer : Component, BreakableComponent
{
    public WarningBuzzer()
    {
        repairCost = 0.5f;
        type = ComponentType.WARNING_BUZZER;
    }
}

public class AlarmLight : Component, BreakableComponent
{
    public AlarmLight()
    {
        repairCost = 0.5f;
        type = ComponentType.ALARM_LIGHT;
    }
}
public class AlarmBuzzer : Component, BreakableComponent
{
    public AlarmBuzzer()
    {
        repairCost = 0.5f;
        type = ComponentType.ALARM_BUZZER;
    }
}


public class ScreenComponent : Component, BreakableComponent
{
    public ScreenComponent()
    {
        repairCost = 1;
        type = ComponentType.SCREEN;
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

public class Pump : Component, BreakableComponent
{
    public Pump()
    {
        repairCost = 3;
        type = ComponentType.PUMP;
    }
}

public class CO2Tank : Component, BreakableComponent
{
    public CO2Tank()
    {
        repairCost = 2.5f;
        type = ComponentType.CO2_TANK;
    }
}

public class NitrogenTank : Component, BreakableComponent
{
    public NitrogenTank()
    {
        repairCost = 2.5f;
        type = ComponentType.NITROGEN_TANK;
    }
}

public class Filter : Component, BreakableComponent
{
    public int grime;
    public Filter()
    {
        repairCost = 9999; //IsBroken is if it is too dirty not if it is broken
        type = ComponentType.FILTER;
        grime = 0;
    }

    public void clean() //To be called to clean the filter
    {
        grime = 0;
        Debug.Log("Filter Cleaned");
        isBroken = false;
    }

    public void filterAir() //while filtering the air, the filter will build up grime.
    {
        grime += 1;
        if(grime >= 100)
        {
            isBroken = true;
        }
    }

    public override void damage()
    {
        grime += 10;
        if (grime >= 100)
        {
            isBroken = true;
        }
    }

}