using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDebugInfo : MonoBehaviour
{
    const float COFF = 0.1f;

    [SerializeField]
    private TextMeshProUGUI ui;

    [SerializeField]
    private float deltaTime = 0f;    

    private Dictionary<string, string> logInfo = new();


    private static ShowDebugInfo showDebug;

    // 싱글톤 인스턴스를 가져오는 프로퍼티
    public static ShowDebugInfo Instance
    {
        get
        {
            if (showDebug == null)
            {
                // 씬에서 싱글톤 인스턴스를 찾거나 생성
                showDebug = FindObjectOfType<ShowDebugInfo>();

                if (showDebug == null)
                {
                    // 씬에 없다면 새로 생성
                    GameObject obj = new GameObject("ShowDebugInfo");
                    showDebug = obj.AddComponent<ShowDebugInfo>();
                }
            }

            return showDebug;
        }
    }

    private void Awake()
    {        
        logInfo.Clear();
    }

    private void Update()
    {
        // 프레임당 경과 시간을 계산합니다.
        deltaTime += (Time.deltaTime - deltaTime) * COFF;
    }

    public void SetLogItem(string logKey, string logValue)
    {
        string outLogValue;
        if (logInfo.TryGetValue(logKey, out outLogValue))
            logInfo[logKey] = logValue;
        else
            logInfo.Add(logKey, logValue);
    }

    private void OnGUI()
    {
        // FPS를 계산하고 화면에 표시합니다.
        float fps = 1.0f / deltaTime;
        string fpsText = string.Format("FPS: {0:F2}", fps);

        string log = "";
        foreach (KeyValuePair<string, string> kv in logInfo)
        {
            log += $"{kv.Key} : {kv.Value}\n";
        }

        GUIStyle style = new();
        style.fontSize = 30;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), fpsText, style);

        ui.SetText(log);
    }
}
