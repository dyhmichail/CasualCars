using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


public class CarGoal : MonoBehaviour
{
    [SerializeField] private Car carToArrive;

    private Collider collider;
    void Start()
    {
        WayPointsManager.inst.OnStartMoving += Activate;
        collider = GetComponent<Collider>();
    }

    void Activate()
    {
        collider.enabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Car")
            CheckCar(collider.gameObject.GetComponent<Car>());
    }

    [SerializeField]
    GameObject carMock;
    void CheckCar(Car car)
    {
        if (car == carToArrive)
        {
            car.gameObject.SetActive(false);
            carMock.SetActive(true);
            GameManager.inst.CarsArrived();
            GetComponent<DOTweenAnimation>().DOPlay();
            WayPointsManager.inst.DeleteRoute(car);
        }
    }
}
