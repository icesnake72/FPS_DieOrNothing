using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDebugInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ui;

    private Dictionary<string, string> logInfo = new();    

    private void Awake()
    {        
        logInfo.Clear();
    }

    private void Update()
    {
        string log = "";
        foreach(KeyValuePair<string, string> kv in logInfo)
        {
            log += $"{kv.Key} : {kv.Value}\n";            
        }

        ui.SetText(log);
    }

    public void SetLogItem(string logKey, string logValue)
    {
        string outLogValue;
        if (logInfo.TryGetValue(logKey, out outLogValue))
            logInfo[logKey] = logValue;
        else
            logInfo.Add(logKey, logValue);
    }
}
