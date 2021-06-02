using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public float waitToStart = 0;
    public float tickTime = 10;
    public LifeSupport lifeSupport;
    [Space(10)]
    public int powerSurgeFusePercentage = 90;

    SystemScript[] systems;

    // Start is called before the first frame update
    void Start()
    {
        systems = Object.FindObjectsOfType<SystemScript>();
        
        InvokeRepeating("Run", waitToStart, tickTime);
    }

    void Run()
    {
        int eventRoll = Random.Range(1, 8); //Get a random number between (inclusive) 1 and 7.


        //Check what event it falls into
        //1. Falling Apart: Break a random component in a random system
        if(eventRoll == 1)
        {
            damageRandomComponent();
        }
        //2. Power Surge: Break a random fuse connected to the power network (Or, less likely, a power connector)
        else if (eventRoll == 2)
        {
            powerSurge();
        }
        //3. Shade: Cause Solar pannels to stop working    [NOT IMPLEMENTED]
        else if (eventRoll == 3)
        {
            Debug.Log("Event Roll, shade");
            //Do something, for now just break something
            damageRandomComponent();
        }
        //4. Combustion: Loose the game if O2 percentage is above 50%
        else if (eventRoll == 4)
        {
            Debug.Log("Event Roll, spark");
            if (lifeSupport.pO2 > 50f)
            {
                lifeSupport.die("Combustion");
            }
            else //If it doesn't kill, cause a power surge
            {
                powerSurge();
            }
        }
        //5-7 Nothing happens
        else
        {
            Debug.Log("Event Roll, nothing happens");
        }

    }


    public void damageRandomComponent() //Damages a random component from all systems
    {
        SystemClass system = systems[Random.Range(0, systems.Length)].system;
        damageRandomComponent(system);
    }
    public void damageRandomComponent(SystemClass system) //Damages a random component from a given system
    {
        Component component = system.systemComponents[Random.Range(0, system.systemComponents.Count)];
        Debug.Log("Damaged " + component.type + " from " + system);
        component.damage();
    }

    public void powerSurge()
    {
        SystemClass system = systems[Random.Range(0, systems.Length)].system;
        if (system.isConnected())
        {
            int powerRoll = Random.Range(1, 101); //Get a random number between 1 and 100 (inclusive)
            if (powerRoll < powerSurgeFusePercentage) //Damage Fuse (or if no fuse, some other part)
            {
                if (system.hasComponent(ComponentType.FUSE)) system.getComponent(ComponentType.FUSE).damage();
                else damageRandomComponent(system);
            }
            else //Damage power connector
            {
                system.getComponent(ComponentType.POWER_CONNECTOR).damage();
            }
        }
    }
}
