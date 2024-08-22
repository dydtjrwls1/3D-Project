using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 이 클래스는 제네릭 타입이다.
// 이 클래스의 T 타입은 Component 를 상속받은 타입만 가능하다.
public class SingleTon<T> : MonoBehaviour where T : Component
{
    // 이 싱글톤이 초기화 된 적이 있는지를 기록하는 변수
    private bool isInitialized = false;

    // 종료 처리에 들어갔는지 확인용 변수
    // static 멤버 함수 안에서는 static 멤버 변수만 접근 가능하다.
    private static bool isShutdown = false;

    private static T instance = null;

    /// <summary>
    /// 싱글톤에 접근하기 위한 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            if (isShutdown) // 종료 처리에 들어간 상황이면
            {
                Debug.LogWarning("싱글톤이 삭제 중에 요구받음.");   // 경고 출력
                return null;                                        // null 리턴
            }

            if (instance == null)
            {
                T singleton = FindAnyObjectByType<T>(); // 일단 씬에 싱글톤이 있는지 찾는다.
                if (singleton == null)
                {
                    // 씬에 싱글톤이 없음
                    GameObject obj = new GameObject();   // Transform 하나만 들어있는 빈 gameobject 생성
                    obj.name = $"{typeof(T)}_Singleton"; // 싱글톤의 게임 오브젝트의 이름 지정하기
                    singleton = obj.AddComponent<T>();   // 싱글톤 게임 오브젝트에 싱글톤용 컴포넌트 추가 
                }
                instance = singleton; // 찾은 것이나 만든 것을 저장하기
                DontDestroyOnLoad(instance.gameObject); // 씬이 닫히더라도 싱글톤용 게임 오브젝트는 유지하기 (삭제되지 않게 하기)
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            // 첫번째로 만들어진 싱글톤
            instance = this as T; // this 를 T 타입으로 캐스팅. 캐스팅이 안되면 null
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            // 이미 만들어진 싱글톤이 있는 상황
            if(instance != this) // 이미 만들어진 것이 자신이 아니면 
            {
                Destroy(this.gameObject); // 스스로 없어지기
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로딩되었을 때 OnSceneLoaded 함수를 실행하도록 등록.
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 등록 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isInitialized)
        {
            // 한 번도 초기화 되지 않았으면
            OnPreInitialize(); // 한 번만 실행하는 초기화 함수 실행
        }
        if(mode != LoadSceneMode.Additive)
        {
            // Additive 모드로 씬이 로딩되지 않았다면
            OnInitialize(); // 반복 초기화 함수 실행
        }
    }

    /// <summary>
    /// 싱글톤이 만들어졌을 때 단 한번만 호출되는 함수
    /// </summary>
    protected virtual void OnPreInitialize()
    {
        isInitialized = true;
    }

    /// <summary>
    /// 싱글톤이 만들어지고 씬이 변경될 때마다 호출될 함수(Additive는 안됨)
    /// </summary>
    protected virtual void OnInitialize()
    {

    }

    private void OnApplicationQuit()
    {
        isShutdown = true;
    }
}

// 싱글톤 : 생성되는 객체가 무조건 1개인 클래스
public class TestSingleTon
{
    private static TestSingleTon instance = null; // 공용으로 사용되는 곳에 인스턴스의 참조를 저장

    public static TestSingleTon Instance
    {
        get
        {
            if (instance == null) // 이전에 인스턴스가 만들어진 적이 없으면 
            {
                instance = new TestSingleTon(); // 인스턴스 생성
            }
            return instance; // 있던 것이나 새로 만들어진 것을 리턴
        }
    }

    private TestSingleTon() // 모든 생성자는 public 이 아니어야 한다.
    {

    }
}

public class TestClass
{
    // static 멤버 변수는 모든 객체에서 공용으로 사용된다.
    public static int staticNumber = 0;

    // 생성자 : 객체가 생성될 때(new 될 때) 실행되는 코드
    // 생성자는 여러개를 만들 수 있다. (파라메터는 달라야함)
    // 생성자를 하난도 작성하지 않으면 기본생성자가 자동으로 생성된다.
    public TestClass() // 기본 생성자
    {
        Debug.Log("TestClass 생성자");
    }

    public TestClass(int i)
    {
        Debug.Log($"TestClass 생성자 실행 : {i}");
    }

    public void TestPrint()
    {
        Debug.Log(staticNumber);
    }
}