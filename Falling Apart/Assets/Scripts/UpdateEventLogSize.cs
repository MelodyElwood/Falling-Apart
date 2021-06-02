using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateEventLogSize : MonoBehaviour
{
    public GameObject eventLog;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("UpdateParentLayoutGroup", 0.1f);
    }

    void UpdateParentLayoutGroup()
    {
        eventLog.gameObject.SetActive(false);
        eventLog.gameObject.SetActive(true);
    }
}
