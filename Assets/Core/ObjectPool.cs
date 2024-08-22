using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀
// 특정 종류의 오브젝트(prefab)를 시작 시점에 대량으로(poolSize) 생성하고 요청받았을 때 하나씩 제공하는 클래스
public class ObjectPool<T> : MonoBehaviour where T : RecycleObject // T 는 반드시 RecycleObject를 상속받은 오브젝트다.
{
    // 풀에서 관리할 게임 오브젝트의 프리펩
    public GameObject prefab;

    // 풀의 크기, 처음에 생성하는 오브젝트의 개수(크기는 2^n 이 좋다)
    public int poolSize = 64;

    // 생성된 모든 오브젝트가 들어있는 배열. (제네릭 T타입으로 해서 다양한 오브젝트를 지원)
    T[] pool;

    // 현재 사용가능한 오브젝트들을 관리하는 큐(pool 배열에서 비활성화 되어 있는 오브젝트만 들어있는 자료구조)
    Queue<T> readyQueue;

    /// <summary>
    /// 초기화용 함수
    /// </summary>
    public virtual void Initialize()
    {
        if (pool == null)
        {
            // 완전히 처음 풀을 만드는 경우. 풀 생성
            pool = new T[poolSize];
            readyQueue = new Queue<T>(poolSize);    // 자료구조를 사용할 때 최대 크기를 알고 있다면 capacity를 설정하는게 좋다.

            GenerateObjects(0, poolSize, pool);
        }
        else
        {
            // 풀이 이미 만들어져 있는 상태 (씬 다시 시작되거나, 다른 씬이 추가로 로딩되거나 등등)
            foreach (T obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 풀에서 사용할 오브젝트를 생성하는 함수
    /// </summary>
    /// <param name="start">새로 생성 시작 할 인덱스</param>
    /// <param name="end">새로 생성이 끝나는 인덱스 + 1</param>
    /// <param name="result">생성된 오브젝트가 들어갈 배열(입력 겸 출력용)</param>
    void GenerateObjects(int start, int end, T[] result)
    {
        for (int i = start; i < end; i++)
        {
            GameObject obj = Instantiate(prefab, transform); // pool의 자식으로 오브젝트 생성
            obj.name = $"{prefab.name}_{i}";    // 이름 변경

            T comp = obj.GetComponent<T>();
            // comp.onDisable += DisableAction; // OnDisable "상황" 일 때 DisableAction 이란 "행동" 을 한다
            comp.onDisable += () =>     // 람다함수를 comp의 onDisable 델리게이트에 등록 = comp 가 비활성화 되면 아래 코드가 실행된다.
            {
                readyQueue.Enqueue(comp); // 레디큐에 컴포넌트 추가해 놓기
            };
            OnGenerateObject(comp);

            result[i] = comp; // 배열에 만들어진 것을 모두 저장
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// 오브젝트 하나가 생성되었을 때 실행될 함수
    /// </summary>
    /// <param name="comp">컴포넌트</param>
    protected virtual void OnGenerateObject(T comp)
    {

    }
    //void DisableAction()
    //{
    //    readyQueue.Enqueue(comp); // 스코프가 맞지 않다
    //}

    /// <summary>
    /// 풀에서 비활성중인 오브젝트를 하나 꺼낸 후 리턴하는 함수
    /// </summary>
    /// <param name="position">배치 될 위치(월드좌표)</param>
    /// <param name="eulerAngle">초기 회전</param>
    /// <returns>활성화된 오브젝트</returns>
    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        if (readyQueue.Count > 0)
        {
            // 아직 비활성화된 오브젝트가 남아있다.
            T comp = readyQueue.Dequeue();                                                  //
            comp.transform.position = position.GetValueOrDefault();                         //
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());     //
            comp.gameObject.SetActive(true);                                                //

            return comp;
        }
        else
        {
            // 모든 오브젝트가 활성화되어 있다. => 남아있는 오브젝트가 없다.
            ExpandPool();
            return GetObject(position, eulerAngle);
        }
    }

    /// <summary>
    /// 풀을 두배로 확장시키는 함수
    /// </summary>
    void ExpandPool()
    {
        // 최대한 실행되지 않아야 함.
        Debug.LogWarning($"{gameObject.name} 풀 사이즈 증가. {poolSize} -> {poolSize * 2}");

        int newSize = poolSize * 2;             // 새 풀의 크기 설정
        T[] newPool = new T[newSize];           // 새 풀의 배열 만들기
        for (int i = 0; i < poolSize; i++)
        {
            newPool[i] = pool[i];               // 이전 풀의 내용을 새 풀에 저장
        }

        GenerateObjects(poolSize, newSize, newPool);    // 새 풀의 남은 부분에 오브젝트 생성해서 추가

        pool = newPool;         // 새 풀의 크기 저장
        poolSize = newSize;     // 새 풀을 정식 풀로 지정
    }
}
