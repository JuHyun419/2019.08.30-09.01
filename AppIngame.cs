using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class AppIngame : MonoBehaviour
{
    private static AppIngame _instance;

    public Text _textVehicleMovement;
    public Transform _transformTrafficSensor;
    public Slider _sliderCongestedAreas;
    public GameObject _gameObjectLogMessage;

    public static bool _IsFastVehicleMovement = false;
    public static float _fCongestedAreasValue = 0.0f;


    private int _iCongestedAreasVehicleCount = 0;
    private float _fCongestedAreasVehicleAverageCurrentSpeed = 0.0f;
    private float _fCongestedAreasVehicleAverageMaxSpeed = 0.0f;
    private int _iCongestedAreasVehicleMax = 12;

    private List<string> _strLogMessage = new List<string>();


    //===============================================================================================
    public static AppIngame Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (AppIngame)FindObjectOfType(typeof(AppIngame));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "AppIngameContainer";
                    _instance = container.AddComponent(typeof(AppIngame)) as AppIngame;
                }
                //DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _textVehicleMovement.text = "디폴트";


        StartCoroutine(coLogMessage());
    }

    // Update is called once per frame
    void Update()
    {
        OnCheckCongestedAreasVehicle();
    }

    public void OnClickVehicleMovement()
    {
        if (_IsFastVehicleMovement == true)
        {
            _IsFastVehicleMovement = false;
            _textVehicleMovement.text = "디폴트";
        }
        else
        {
            _IsFastVehicleMovement = true;
            _textVehicleMovement.text = "알고리즘 적용";
        }
    }

    private void OnCheckCongestedAreasVehicle()
    {
        Vector3 vWorldPosition = _transformTrafficSensor.TransformPoint(Vector3.zero);

        Collider[] collider = Physics.OverlapSphere(vWorldPosition, 30.0f);

        _iCongestedAreasVehicleCount = 0;
        _fCongestedAreasVehicleAverageCurrentSpeed = 0.0f;
        _fCongestedAreasVehicleAverageMaxSpeed = 0.0f;
        if (collider.Length > 0)
        {
            for (int i = 0; i < collider.Length; ++i)
            {
                if (collider[i].gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    ++_iCongestedAreasVehicleCount;
                    VehicleController vehicleController = collider[i].GetComponent<VehicleController>();
                    if (vehicleController)
                    {
                        _fCongestedAreasVehicleAverageCurrentSpeed += vehicleController.GetCurrentSpeed();
                        _fCongestedAreasVehicleAverageMaxSpeed += vehicleController.GetMaxSpeed();
                    }
                }
            }

            if (_iCongestedAreasVehicleCount > 0)
            {
                _fCongestedAreasVehicleAverageCurrentSpeed = _fCongestedAreasVehicleAverageCurrentSpeed / _iCongestedAreasVehicleCount;
                _fCongestedAreasVehicleAverageMaxSpeed = _fCongestedAreasVehicleAverageMaxSpeed / _iCongestedAreasVehicleCount;
            }
            else
            {
                _fCongestedAreasVehicleAverageCurrentSpeed = 1.0f;
                _fCongestedAreasVehicleAverageMaxSpeed = 1.0f;
            }
        }

        _fCongestedAreasValue = GetCongestedAreasValue();
        Vector3 vA = new Vector3(_sliderCongestedAreas.value, 0.0f, 0.0f);
        Vector3 vB = new Vector3(_fCongestedAreasValue, 0.0f, 0.0f);
        Vector3 vResult = Vector3.Lerp(vA, vB, Time.deltaTime * 2.0f);
        _sliderCongestedAreas.value = vResult.x;
    }

    public float GetCongestedAreasValue()
    {
        float fResultValue = 0.0f;
        float fCountRate = (float)_iCongestedAreasVehicleCount / (float)_iCongestedAreasVehicleMax;
        fCountRate = Mathf.Clamp01(fCountRate);
        float fSpeedRate = _fCongestedAreasVehicleAverageCurrentSpeed / _fCongestedAreasVehicleAverageMaxSpeed;
        fSpeedRate = Mathf.Clamp01(fSpeedRate);
        fSpeedRate = Mathf.Pow(1.0f - fSpeedRate, 2.0f);

        fResultValue = (fCountRate * 0.55f) + (fSpeedRate * 0.45f);
        fResultValue = Mathf.Clamp01(fResultValue);
        return fResultValue;
    }
    /*
    public float GetCongestedAreasValue()
    {
        float fResultValue = 0.0f;
        float fCountRate = (float)_iCongestedAreasVehicleCount / (float)_iCongestedAreasVehicleMax;
        fCountRate = Mathf.Clamp01(fCountRate);
        float fSpeedRate = _fCongestedAreasVehicleAverageCurrentSpeed / _fCongestedAreasVehicleAverageMaxSpeed;
        fSpeedRate = Mathf.Clamp01(fSpeedRate);


        fResultValue = fCountRate;
        float fMinValue = 0.1f;
        float fMaxValue = 1.0f;
        fResultValue = Mathf.Clamp(fResultValue, fMinValue, fMaxValue);
        fResultValue = Mathf.Clamp01(fResultValue - fMinValue) / fMaxValue - fMinValue;

        if (fSpeedRate <= 0.4f)
        {
            fResultValue += 0.4f;
            fResultValue = Mathf.Clamp01(fResultValue);
        }

        return fResultValue;
    }
    */
    public void OnClickExit()
    {
        Application.Quit();
    }

    IEnumerator coLogMessage()
    {
        while (true)
        {
            int iMessageType = UnityEngine.Random.Range(0, 3);
            //int iMessageType = 2;
            switch (iMessageType)
            {
                case 0:
                    {
                        float fValue = _fCongestedAreasValue;
                        fValue = Mathf.Clamp01(fValue);
                        string strData = "차량 정체 수치 : " + fValue.ToString("0.0");
                        AddLogMessage(strData);
                    }
                    break;
                case 1:
                    {
                        int iValue = _iCongestedAreasVehicleCount;
                        iValue = Mathf.Clamp(iValue, 0, _iCongestedAreasVehicleMax);
                        string strData = "정체 감지 센서 영역 차량 대수 : " + iValue.ToString();
                        AddLogMessage(strData);
                    }
                    break;
                case 2:
                    {
                        if (_iCongestedAreasVehicleCount > 0)
                        {
                            float fValue = (_fCongestedAreasVehicleAverageCurrentSpeed * 60.0f * 60.0f) / 1000.0f;
                            fValue = Mathf.Clamp(fValue, 0.0f, 200.0f);
                            string strData = "정체 감지 센서 영역 차량 평균속도 : " + fValue.ToString("0.0") + "km/h";
                            AddLogMessage(strData);
                        }
                    }
                    break;
            }

            float fDelayTime = UnityEngine.Random.Range(0.2f, 0.5f);
            yield return new WaitForSeconds(fDelayTime);
        }
    }

    private void AddLogMessage(string strMessage)
    {
        if (_strLogMessage.Count > 20)
        {
            _strLogMessage.RemoveAt(0);
        }

        // >>>>> add
        _strLogMessage.Add(strMessage);

        UpdateLogMessage();
    }

    private void UpdateLogMessage()
    {
        StringBuilder strBuild = new StringBuilder();

        for (int i = _strLogMessage.Count - 1; i >= 0; --i)
        {
            strBuild.Append(" " + _strLogMessage[i] + Environment.NewLine);
        }

        Text textLog = _gameObjectLogMessage.GetComponent<Text>();
        if (textLog)
        {
            textLog.text = strBuild.ToString();
        }
        int qwer = 2134;
    }
}
