using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SystemType { OXYGEN_GENERATOR, CO2_SCRUBBER, PRESSURIZER, SOLAR_PANELS, BATTERY_CHARGER, MAIN_LIGHTS, BACKUP_LIGHTS, MAIN_COMPUTER };


public class SystemScript : MonoBehaviour
{
    public SystemType systemType;

    public System system;
    // Start is called before the first frame update
    void Start()
    {
        switch (systemType)
        {
            case SystemType.OXYGEN_GENERATOR:
                system = new Fuse();
                break;
            case SystemType.CO2_SCRUBBER:
                system = new Battery();
                break;
            case SystemType.PRESSURIZER:
                system = new WarningLight();
                break;
            case SystemType.SOLAR_PANELS:
                //system = new WarningBuzzer();
                break;
            default:
                Debug.LogError("Unkown System Type", this);
                break;
        }
    }
}

public abstract class System
{

}
