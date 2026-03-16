using System.Collections.Generic;
using UnityEngine;
using CherryFramework.DependencyManager;

public class SimplePool : MonoBehaviour
{
    public static SimplePool Instance { get; private set; }

    private readonly Dictionary<int, Queue<GameObject>> _pool = new(); // ID префаба -> Очередь неактивных (свободных) объектов
    
    private readonly Dictionary<int, int> _instanceToPrefabId = new(); // ID инстанса -> ID его оригинального префаба (чтобы знать, в какую очередь возвращать)

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int prefabId = prefab.GetInstanceID();
        

        if (!_pool.ContainsKey(prefabId))  // есть ли для него очередь
        {
            _pool[prefabId] = new Queue<GameObject>();
        }

        GameObject obj;
        
        if (_pool[prefabId].Count > 0) // берем из очереди, если есть неактивные
        {
            obj = _pool[prefabId].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else // если нет - создаем новый
        {
            obj = Instantiate(prefab, position, rotation);
            
            // прокидываем зависимости CherryFramework в новый объект
            DependencyContainer.Instance.InjectDependencies(obj);
            
            // запоминаем, от какого префаба произошел этот объект
            _instanceToPrefabId[obj.GetInstanceID()] = prefabId;
        }

        return obj;
    }


    // Дженерик-версия для удобного получения сразу нужного компонента
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = Spawn(prefab.gameObject, position, rotation);
        return obj.GetComponent<T>();
    }


    // Возвращает объект обратно в пул
    public void Despawn(GameObject obj)
    {
        if (!obj.activeSelf) return;

        int instanceId = obj.GetInstanceID();
        
        // Проверяем, был ли этот объект создан через наш пул
        if (_instanceToPrefabId.TryGetValue(instanceId, out int prefabId))
        {
            obj.SetActive(false);
            _pool[prefabId].Enqueue(obj);
        }
        else
        {
            // Если кто-то попытался задеспавнить объект, созданный обычным Instantiate
            Debug.LogWarning($"[SimplePool] Объект {obj.name} не принадлежит пулу. Он будет уничтожен.");
            Destroy(obj);
        }
    }
}