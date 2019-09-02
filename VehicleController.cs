using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 유니티 상에서 스크립트를 생성하게 되면 항상 MonoBehaviour를 상속받음
 * 모든 스크립트의 기본이 되는 클래스, 유니티 에디터에서 스크립트를 생성하면 자동으로 상속됨
 * On 접두사로 시작하는 이름의 함수들을 가지고 있다.
 * OnGUI, OnMouseEnter, OnMouseDown 등등
 */
public class VehicleController : MonoBehaviour
{
    // Transform클래스 = position, rotation, scale 나타내는 클래스
    // 각각 위치, 회전, 크기(확대 비율)을 나타냄
    public Transform _transformFront;
    public Transform _transformBack;

    /* enum => 열거형 (함수와 비슷하게 사용)
     * enum 이름 = enTrackLocation 
     * 사용할때 => enTrackLocation.Left 식으로 사용가능(Left, Right, Max 등등)
     * 자동으로 0, 1, 2, 3... 등이 대입된다.
     */
    public enum enTrackLocation
    {
        Left,
        Right,
        Max
    }

    // enum 열거형
    public enum enSpeedType
    {
        Low,
        Middle, 
        High,
        Max
    }
    
    // 구조체
    public struct sSteeringData
    {
        public float fMaxSpeed;
        public float fCurrentSpeed;
        public float fForce;
        public float fAccelSpeed;
        public float fBreakSpeed;
        public float fBreakTimeDistance;
    }

    private sSteeringData _sMySteeringData = new sSteeringData();       // 구조체
    private enSpeedType _enSpeedType = enSpeedType.Low;                 // enSpeedType 얼거형의 Low 값을 _enSpeedType로 설정
    private enTrackLocation _enTrackLocation = enTrackLocation.Left;    // enTrackLocation 열거형의 Left 값을 _enTrackLocation로 설정


    // Start is called before the first frame update
    // 스크립트 초기화될때 호출됨
    // 스크립트가 enable여야 호출된다.
    void Start()
    {
        SetSteeringData();
    }

    // SetTrackLocation 함수 => newTrackLocation(매개변수)를 Left로 지정
    public void SetTrackLocation(enTrackLocation newTrackLocation)  // enum 변수명
    {
        _enTrackLocation = newTrackLocation;
    }

    public float GetCurrentSpeed()
    {
        // 구조체의 fCurrentSpeed 값 리턴
        return _sMySteeringData.fCurrentSpeed;
    }

    public float GetMaxSpeed()
    {
        // 구조체의 fMaxSpeed 값 리턴
        return _sMySteeringData.fMaxSpeed;
    }

    private void SetSteeringData()
    {
        // iFlag를 0, 100사이의 랜덤값으로 설정
        int iFlag = Random.Range(0, 100);
        if (iFlag > 70)     // 랜덤값이 70 초과일때
        {     
            _enSpeedType = enSpeedType.Low;
        }
        else    // 랜덤 값이 70 미만일때
        {
            // iValue의 랜덤값 설정,
            int iValue = Random.Range((int)enSpeedType.Middle, (int)enSpeedType.Max);
            _enSpeedType = (enSpeedType)iValue;
        }

        // 위에서 설정한 _enSpeedType의 값에대한 케이스
        switch (_enSpeedType)
        {
            case enSpeedType.Low:
                {
                    // 구조체의 변수들 설정
                    _sMySteeringData.fMaxSpeed = Random.Range(11.0f, 13.0f);
                    _sMySteeringData.fAccelSpeed = Random.Range(3.5f, 4.5f);
                    _sMySteeringData.fBreakSpeed = Random.Range(1.0f, 1.5f);
                    // 정지 할때의 각자 거리
                    _sMySteeringData.fBreakTimeDistance = Random.Range(2.5f, 5.0f);
                }
                break;
            case enSpeedType.Middle:
                {
                    // 구조체의 변수들 설정
                    _sMySteeringData.fMaxSpeed = Random.Range(16.0f, 20.0f);
                    _sMySteeringData.fAccelSpeed = Random.Range(6.0f, 8.0f);
                    _sMySteeringData.fBreakSpeed = Random.Range(1.5f, 2.0f);
                    // 정지 할때의 각자 거리
                    _sMySteeringData.fBreakTimeDistance = Random.Range(4.0f, 6.0f);
                }
                break;
            case enSpeedType.High:
                {
                    // 구조체의 변수들 설정
                    _sMySteeringData.fMaxSpeed = Random.Range(25.0f, 30.0f);
                    _sMySteeringData.fAccelSpeed = Random.Range(8.0f, 10.0f);
                    _sMySteeringData.fBreakSpeed = Random.Range(2.0f, 3.0f);
                    // 정지 할때의 각자 거리
                    _sMySteeringData.fBreakTimeDistance = Random.Range(5.0f, 8.0f);
                }
                break;
        }

        // 0으로 초기화
        _sMySteeringData.fForce = 0.0f;
        _sMySteeringData.fCurrentSpeed = 0.0f;
    }

   /*
    * Update is called once per frame
    * 매 프레임마다 호출되는 유니티 기본함수
    * 초당 30프레임 => 1초에 Update()가 30번 호출
    * 지속적으로 처리되거나 실시간으로 이뤄지는 변화처리에 사용
    * 일반적으로 게임 행위를 처리할 때 사용(ex 키 입력 받을 때)
    */
    void Update()
    {
        OnMoveVehicle();        // 차량 움직임을 나타내는 메서드
        OnDestoryVehicle();     // 차량 움직임을 제거하는 메서드
    }

    // 차량 움직임을 나타내는 메서드
    private void OnMoveVehicle()       
    {
        // start steering
        OnStartSteeringVehicle();

        // Translate : Moves the transform in the direction and distance of translation.
        // transform.position  => 고정 좌표 이동(귀환, 텔레포트 등)
        // transform.Translate => 상대 좌표 이동(점프, 앞뒤로 이동하는 것)
        transform.Translate(new Vector3(0.0f, 0.0f, _sMySteeringData.fCurrentSpeed * Time.deltaTime));
        OnSteeringRotation();      // 바로 밑에서 설명

        // end steering 
        OnEndSteeringVehicle();
    }
    
    /* AppIngame._IsFastVehicleMovement => 메인 화면(시뮬레이션 영상)에서 '디폴트', '알고리즘 적용'의 텍스트가 있음
     * 텍스트에서 true는 디폴트값임(즉 젤첨에 실행했을때 나오는 화면)
     */
    private void OnSteeringRotation()
    {
        if (AppIngame._IsFastVehicleMovement == true)
        {
            if (_sMySteeringData.fCurrentSpeed > 0.0f)
            {
                if (_enTrackLocation == enTrackLocation.Left)
                {

                }
                else
                {

                }
            }
        }
    }

    // 차량 조절 메서드(시작)
    private void OnStartSteeringVehicle()
    {
        OnCalculateForce();     // 차량의 속도를 계산(조절)하는 메서드

        //  최대, 최소 속도 제한( 제작자분이 최대속도 60으로 제한해놨음 )
        // 주석좀 달아놓지 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        _sMySteeringData.fCurrentSpeed += _sMySteeringData.fForce;
        _sMySteeringData.fCurrentSpeed = Mathf.Clamp(_sMySteeringData.fCurrentSpeed, 0.0f, _sMySteeringData.fMaxSpeed);
    } 

    // 차량의 속도를 계산(조절)하는 메서드
    private void OnCalculateForce()
    {
        float fDistanceOtherVehicle = FindApproachOtherVehicle();       // 차량에 접촉한 다른 차가 있는지 검사하는 메서드
        float fDistanceTrafficLight = FindApproachTrafficLight();
        if (fDistanceOtherVehicle == -1 && fDistanceTrafficLight == -1)
        {
            _sMySteeringData.fForce += _sMySteeringData.fAccelSpeed * Time.deltaTime;
        }
        else
        {
            float fDistance = fDistanceOtherVehicle;
            if (fDistanceOtherVehicle > fDistanceTrafficLight)
            {
                fDistance = fDistanceTrafficLight;
            }

            float fWeight = _sMySteeringData.fBreakTimeDistance - fDistance;
            fWeight = Mathf.Pow(fWeight, 2.0f);
            _sMySteeringData.fForce -= _sMySteeringData.fBreakSpeed * Time.deltaTime * fWeight;
        }
    }

    // 차량에 접촉한 다른 차가 있는지 검사하는 메서드
    private float FindApproachOtherVehicle()
    {
        /*
         * ollider = 충돌체의 정보 저장
         * ray = 광선의 정보를 담음
         * 객체 생성
         * _transformfront.position => 고정좌표(귀환, 텔레포트 등과 같은 것)
         */
        
        Ray ray = new Ray(_transformFront.position, Vector3.forward);

        // 충돌하는 오브젝트 검출하는 메서드
        RaycastHit[] hit = Physics.RaycastAll(ray, _sMySteeringData.fBreakTimeDistance);
        for (int i = 0; i < hit.Length; ++i)
        {
            if (gameObject != hit[i].collider.gameObject && hit[i].collider.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                // GetComponent = Returns the component of Type type if the game object has one attached, null if it doesn't.
                // 게임 오브젝트(아마 차량일듯?)가 한번 접촉될때, 리턴한다네,, 접촉안되면 null
                VehicleController otherVehicle = hit[i].collider.GetComponent<VehicleController>();
                if (otherVehicle)
                {
                    float fDistance = Vector3.Distance(_transformFront.position, otherVehicle._transformBack.position);
                    return fDistance;
                }
            }
        }

        return -1;
    }


    private float FindApproachTrafficLight()
    {
        // Vector3 = 3차원 벡터와 위치 표현
        
        // TrafficLightController cs파일에서 위치 불러옴
        Vector3 vTrafficLightLocation = TrafficLightController._stTransform.position;
        // x값의 위치를 맞춘다.
        vTrafficLightLocation.x = _transformFront.position.x;

        if (TrafficLightController._stenTrafficLightType != TrafficLightController.enTrafficLightType.Green)
        {
            float fDistance = Vector3.Distance(_transformFront.position, vTrafficLightLocation);
            if (_sMySteeringData.fBreakTimeDistance >= fDistance && _transformFront.position.z < TrafficLightController._stTransform.position.z + 0.5f)
            {
                return fDistance;
            }
        }

        return -1;
    }

    // 차량 조절 메서드(끝) => 초기화
    private void OnEndSteeringVehicle()
    {
        _sMySteeringData.fForce = 0.0f;
    }

    // Removes a gameobject, component or asset.
    // 차량을 제거하는 메서드(z, x의 값이 if문의 범위일때)
    private void OnDestoryVehicle()
    {
        if (transform.position.z > 300.0f || transform.position.x < -300.0f || transform.position.x > 300.0f)
        {
            // 파괴 !
            Destroy(gameObject);
        }
    }
}
