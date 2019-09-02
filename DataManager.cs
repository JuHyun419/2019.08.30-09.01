using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;


    //===============================================================================================
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (DataManager)FindObjectOfType(typeof(DataManager));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "DataManagerContainer";
                    _instance = container.AddComponent(typeof(DataManager)) as DataManager;
                }
                //DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public SpawnerVehicle.sVehicleIncreaseInfo CSVReadVehicleIncreaseInfo(string strFileName)
    {
        SpawnerVehicle.sVehicleIncreaseInfo sData = new SpawnerVehicle.sVehicleIncreaseInfo();

        string strPathName = System.IO.Path.Combine(Application.streamingAssetsPath, strFileName);
        //string strPathName = Application.streamingAssetsPath + "/" + strFileName;
        string resourceText = System.IO.File.ReadAllText(strPathName);
        if (resourceText == "")
        {
            Debug.Log("$$$$$(error)CSV : file not found (" + strFileName + ")");
            return sData;
        }

        // 한 라인 씩 저장
        string[] strLines = resourceText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (strLines.Length <= 1)
            return sData;

        for (int i = 0; i < strLines.Length; ++i)
        {
            if (strLines[i] == "" || strLines[i] == " ")
                continue;

            string[] strElementTable = strLines[i].Split(',');

            if (strElementTable.Length == 2)
            {
                if (strElementTable[0] == "increase_width_time")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fWidthTime = fValue;
                }
                if (strElementTable[0] == "increase_height_clamp_min")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fHeightClampMin = fValue;
                }
                if (strElementTable[0] == "increase_height_clamp_max")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fHeightClampMax = fValue;
                }
            }
        }

        return sData;
    }

    public TrafficLightController.sTrafficLight CSVReadTrafficLight(string strFileName)
    {
        TrafficLightController.sTrafficLight sData = new TrafficLightController.sTrafficLight();
        sData.fTrafficLightTime = new float[(int)TrafficLightController.enTrafficLightType.Max];

        string strPathName = System.IO.Path.Combine(Application.streamingAssetsPath, strFileName);
        //string strPathName = Application.streamingAssetsPath + "/" + strFileName;
        string resourceText = System.IO.File.ReadAllText(strPathName);
        if (resourceText == "")
        {
            Debug.Log("$$$$$(error)CSV : file not found (" + strFileName + ")");
            return sData;
        }

        // 한 라인 씩 저장
        string[] strLines = resourceText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (strLines.Length <= 1)
            return sData;

        for (int i = 0; i < strLines.Length; ++i)
        {
            if (strLines[i] == "" || strLines[i] == " ")
                continue;

            string[] strElementTable = strLines[i].Split(',');

            if (strElementTable.Length == 2)
            {
                if (strElementTable[0] == "red_time")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fTrafficLightTime[(int)TrafficLightController.enTrafficLightType.Red] = fValue;
                }
                if (strElementTable[0] == "yellow_time")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fTrafficLightTime[(int)TrafficLightController.enTrafficLightType.Yellow] = fValue;
                }
                if (strElementTable[0] == "green_time")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fTrafficLightTime[(int)TrafficLightController.enTrafficLightType.Green] = fValue;
                }
                if (strElementTable[0] == "calculate_green_time_weight")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fGreenTimeWeight = fValue;
                }
                if (strElementTable[0] == "calculate_green_time_min_value")
                {
                    float fValue = float.Parse(strElementTable[1]);
                    sData.fGreenTimeMinValue = fValue;
                }
            }
        }

        return sData;
    }
    /*
    public static List<Dictionary<string, string>> CSVRead(string strFileName)
    {
        List<Dictionary<string, string>> listData = new List<Dictionary<string, string>>();

        TextAsset resourceText = Resources.Load<TextAsset>(strFileName);
        if (resourceText == null)
        {
            Debug.Log("$$$$$(error)CSV : file not found (" + strFileName + ")");
            return listData;
        }

        // 한 라인 씩 저장
        string[] strLines = resourceText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (strLines.Length <= 1)
            return listData;

        // 키 값 정보
        string[] strKeyTable = strLines[0].Split(',');

        for (int i = 1; i < strLines.Length; ++i)
        {
            if (strLines[i] == "" || strLines[i] == " ")
                continue;

            string[] strElementTable = strLines[i].Split(',');

            Dictionary<string, string> element = new Dictionary<string, string>();

            for (int k = 0; k < strElementTable.Length; ++k)
            {
                element.Add(strKeyTable[k], strElementTable[k]);
            }

            listData.Add(element);
        }

        return listData;
    }*/
    /*
    public static List<TrackManager.sWayPoint> CSVReadByWayPoint(string strFileName)
    {
        List<TrackManager.sWayPoint> listData = new List<TrackManager.sWayPoint>();

        TextAsset resourceText = Resources.Load<TextAsset>(strFileName);
        if (resourceText == null)
        {
            Debug.Log("$$$$$(error)CSV : file not found (" + strFileName + ")");
            return listData;
        }

        // 한 라인 씩 저장
        string[] strLines = resourceText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (strLines.Length <= 1)
            return listData;

        // 키 값 정보
        string[] strKeyTable = strLines[0].Split(',');

        for (int i = 1; i < strLines.Length; ++i)
        {
            if (strLines[i] == "" || strLines[i] == " ")
                continue;

            string[] strElementTable = strLines[i].Split(',');

            TrackManager.sWayPoint element = new TrackManager.sWayPoint();

            // "position"
            string[] strPerPos = strElementTable[1].Split(':');
            if (strPerPos.Length == 3)
            {
                element.vPosition.x = float.Parse(strPerPos[0]);
                element.vPosition.y = float.Parse(strPerPos[1]);
                element.vPosition.z = float.Parse(strPerPos[2]);
            }
            else
            {
                Debug.Log("$$$$$(error) CSVReadByWayPoint() error position");
            }

            // "length"
            element.fSumLength = float.Parse(strElementTable[2]);

            // "degree"
            element.iDegree = int.Parse(strElementTable[2]);


            // >>>>> add
            listData.Add(element);
        }

        return listData;
    }*/
}
