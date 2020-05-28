using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WayPointsManager : SerializedMonoBehaviour
{
    //минимальный шаг на которые двигается машинка
    [SerializeField] private float minimalStepToMove;

    //[SerializeField] private GameObject wayPointSample;
    //эстетическое дополнение, чтобы все объекты были в одной папке и не захламлсяли сцену в плеймоде, привычка
    [SerializeField] private Transform wayPointsFolder;

    private Dictionary<Car, List<GameObject>> currentRoutes = new Dictionary<Car, List<GameObject>>();

    //создаем синглтон
    public static WayPointsManager inst;
    internal Action OnStartMoving;

    void Awake()
    {
        inst = this;
    }

    [Button]
    public void StartMoving()
    {
        foreach (var route in currentRoutes)
        {
            route.Key.GO(route.Value);
        }

        OnStartMoving?.Invoke();
    }

    public void CreateWaypoint(Car car, Vector3 position, GameObject brush)
    {
        //проверяем на дистанцию к предыдущему
        if (distanceIsFarThenMinimal(car, position))
        {
            //добавляем так же и максимальную проверку чтобы не было резких скачков

            //создаем новую точку
            var tempGobj = GameObject.Instantiate(brush, position, Quaternion.identity, wayPointsFolder);

            AddToDict(car, tempGobj);
        }
        
    }

    private void AddToDict(Car car, GameObject wayPoint)
    {
        if (currentRoutes.ContainsKey(car))
            currentRoutes[car].Add(wayPoint);
        else
            currentRoutes.Add(car, new List<GameObject>(){wayPoint});
    }

    private bool distanceIsFarThenMinimal(Car car, Vector3 position)
    {
        if (!currentRoutes.ContainsKey(car))
            return true;

        if (currentRoutes[car].Count < 2)
            return true;

        var prevPosition = currentRoutes[car][currentRoutes[car].Count - 1];
        float distance = Vector3.Distance(prevPosition.transform.position, position);
        if (distance >= minimalStepToMove)
            return true;
        else
            return false;
    }

    public void DeleteRoute(Car car)
    {
        if (currentRoutes.ContainsKey(car))
        {
            for (int i = currentRoutes[car].Count - 1; i >= 0; i--)
                Destroy(currentRoutes[car][i]);

            currentRoutes[car].Clear();
        }
    }
}
