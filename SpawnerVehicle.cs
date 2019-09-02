using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerVehicle : MonoBehaviour
{
    public Slider _sliderVehicleIncrease;

    public struct sVehicleIncreaseInfo
    {
        public float fWidthTime;
        public float fHeightClampMin;
        public float fHeightClampMax;
    }

    // 차량 증가 폭
    public static float _stfVehicleIncreaeHeightRate = 1.0f;
    private sVehicleIncreaseInfo _sVehicleIncreaseInfo;

    private float _fStartDelayTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        InitVehicleSetting();

        StartCoroutine(coSpawnVehicle());
    }

    private void InitVehicleSetting()
    {
        _sVehicleIncreaseInfo = DataManager.Instance.CSVReadVehicleIncreaseInfo("Vehicle_Increase_Info.csv");
        
        // 차량 증가폭
        _sVehicleIncreaseInfo.fWidthTime = Mathf.Clamp(_sVehicleIncreaseInfo.fWidthTime, 1.0f, 100.0f);
    }

    // Update is called once per frame
    void Update()
    {
        _fStartDelayTime -= Time.deltaTime;
        if (_fStartDelayTime <= 0.0f)
        {
            _stfVehicleIncreaeHeightRate = Mathf.Cos(Time.realtimeSinceStartup * (Mathf.PI * (1.0f / _sVehicleIncreaseInfo.fWidthTime)));
            // 0~1값으로 변환
            _stfVehicleIncreaeHeightRate = (_stfVehicleIncreaeHeightRate * 0.5f) + 0.5f;
            // 범위를 설정한다.
            // 최대 값 이상은 최대 값으로 처리
            if (_stfVehicleIncreaeHeightRate >= _sVehicleIncreaseInfo.fHeightClampMax)
            {
                _stfVehicleIncreaeHeightRate = _sVehicleIncreaseInfo.fHeightClampMax;
            }
            // 최소 값 미만은 0으로 처리 : 확률 없음
            if (_stfVehicleIncreaeHeightRate < _sVehicleIncreaseInfo.fHeightClampMin)
            {
                _stfVehicleIncreaeHeightRate = 0.0f;
            }

            // clamp한 값을 0~1로 환산해서 사용한다.
            float fFullWidth = _sVehicleIncreaseInfo.fHeightClampMax - _sVehicleIncreaseInfo.fHeightClampMin;
            if (fFullWidth <= 0.0f)
                fFullWidth = 1.0f;
            float fValue = (_stfVehicleIncreaeHeightRate - _sVehicleIncreaseInfo.fHeightClampMin) / fFullWidth;
            _sliderVehicleIncrease.value = Mathf.Clamp01(fValue);
        }
    }

    IEnumerator coSpawnVehicle()
    {
        while (true)
        {
            float fXOffsetValue = 4.6f;
            for (int i = 0; i < 2; ++i)
            {
                float fXOffset = (fXOffsetValue * i) - (fXOffsetValue * 0.5f);
                if (IsExistSpawnPositionToVehicle(fXOffset) == false && IsSpawnTime() == true)
                {
                    int iValue = Random.Range(1, 6);
                    string strVehicleName = "Vehicle/Vehicle" + iValue.ToString("00");
                    Vector3 vPosition = new Vector3(transform.position.x + fXOffset, transform.position.y, transform.position.z);
                    GameObject go = Instantiate(Resources.Load(strVehicleName), vPosition, transform.rotation) as GameObject;
                    if (go)
                    {
                        VehicleController vehicleController = go.GetComponent<VehicleController>();
                        if (vehicleController)
                        {
                            vehicleController.SetTrackLocation((VehicleController.enTrackLocation)i);
                        }
                    }
                }
            }
 
            yield return new WaitForSeconds(0.4f);
        }
    }

    bool IsSpawnTime()
    {
        int iValue = Random.Range(0, 100);
        if ((float)iValue > 100.0f - (_stfVehicleIncreaeHeightRate * 100.0f))
        {
            return true;
        }

        return false;
    }

    bool IsExistSpawnPositionToVehicle(float fXPosition)
    {
        for (int i = 0; i < 2; ++i)
        {
            Vector3 vStartPosition = new Vector3(fXPosition, 10.0f, transform.position.z + (i * 3.3f));
            Ray ray = new Ray(vStartPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20.0f))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
