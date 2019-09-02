using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightController : MonoBehaviour
{
    public Transform _transformTrafficLight;
    public Slider _sliderTrafficLight;

    public enum enTrafficLightType
    {
        None,
        Red,
        Yellow,
        Green,
        Max
    }

    public struct sTrafficLight
    {
        public float[] fTrafficLightTime;
        public float fGreenTimeWeight;
        public float fGreenTimeMinValue;
    }


    public static Transform _stTransform;
    public static enTrafficLightType _stenTrafficLightType = enTrafficLightType.None;

    private bool _IsSwitchFlag = true;

    private sTrafficLight _sTrafficLight;
    private float[] _fTrafficLightTime;



    // Start is called before the first frame update
    void Start()
    {
        _stTransform = _transformTrafficLight;

        InitTrafficLight();

        StartCoroutine(coTrafficLight());
    }

    private void InitTrafficLight()
    {
        _sTrafficLight = DataManager.Instance.CSVReadTrafficLight("TrafficLight.csv");

        _fTrafficLightTime = new float[(int)enTrafficLightType.Max];
        for (int i = 0; i < (int)enTrafficLightType.Max; ++i)
        {
            _fTrafficLightTime[i] = _sTrafficLight.fTrafficLightTime[i];
        }
    }

    IEnumerator coTrafficLight()
    {
        Color[] matColor = { Color.black, Color.red, Color.yellow, Color.green };

        while (true)
        {
            int iTrafficLightType = (int)_stenTrafficLightType;
            if (_IsSwitchFlag == true)
            {
                ++iTrafficLightType;
                if (iTrafficLightType >= (int)enTrafficLightType.Max)
                {
                    iTrafficLightType = (int)enTrafficLightType.Yellow;
                    _IsSwitchFlag = false;
                }
            }
            else
            {
                --iTrafficLightType;
                if (iTrafficLightType <= (int)enTrafficLightType.None)
                {
                    iTrafficLightType = (int)enTrafficLightType.Yellow;
                    _IsSwitchFlag = true;
                }
            }


            _stenTrafficLightType = (enTrafficLightType)iTrafficLightType;

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                meshRenderer.material.color = matColor[iTrafficLightType];
            }

            yield return new WaitForSeconds(_fTrafficLightTime[iTrafficLightType]);
        }
    }

    private void CalculateTrafficLightGreenTime()
    {
        _fTrafficLightTime[(int)enTrafficLightType.Green] = _sTrafficLight.fTrafficLightTime[(int)enTrafficLightType.Green];

        if (AppIngame._IsFastVehicleMovement == true)
        {
            float fValue = AppIngame._fCongestedAreasValue;
            if (AppIngame._fCongestedAreasValue < SpawnerVehicle._stfVehicleIncreaeHeightRate)
            {
                fValue = SpawnerVehicle._stfVehicleIncreaeHeightRate;
            }
            fValue = (fValue * _sTrafficLight.fGreenTimeWeight) + _sTrafficLight.fGreenTimeMinValue;
            _fTrafficLightTime[(int)enTrafficLightType.Green] += fValue;
        }
    }

    private void CalculateTrafficLightGreenRate()
    {
        float fGreenRate = _fTrafficLightTime[(int)enTrafficLightType.Green] / (_fTrafficLightTime[(int)enTrafficLightType.Green] + _fTrafficLightTime[(int)enTrafficLightType.Red]);

        Vector3 vA = new Vector3(_sliderTrafficLight.value, 0.0f, 0.0f);
        Vector3 vB = new Vector3(fGreenRate, 0.0f, 0.0f);
        Vector3 vResult = Vector3.Lerp(vA, vB, Time.deltaTime * 2.0f);
        _sliderTrafficLight.value = vResult.x;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateTrafficLightGreenTime();
        CalculateTrafficLightGreenRate();
    }
}
