using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [Tooltip("Время с которым машинки проходять вейпоинт, чем оно ниже тем быстрее они двигаются")]
    [SerializeField] private float speed;
    [SerializeField] private carType carType;

    private DOTweenAnimation animation;
    private Rigidbody rigidbody;
    private Collider collider;
    void Start()
    {
        animation = GetComponent<DOTweenAnimation>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    internal void GO(List<GameObject> route)
    {
        StartCoroutine(IEMove(route));
    }

    //[SerializeField] private GameObject goal;

    //[Button]
    //void TestRotate()
    //{
    //    transform.DOLookAt(goal.transform.position, speed);
    //}

    IEnumerator IEMove(List<GameObject> route)
    {
        for (int i = 0; i < route.Count - 1; i++)
        {
            transform.DOMove(route[i+1].transform.position, speed).SetEase(Ease.Linear);
            transform.DOLookAt(route[i+1].transform.position, speed).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(speed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject.tag == "Car" ||
                contact.otherCollider.gameObject.tag == "Obstacle")
            {
                beginCrush = true;

                crashVector = transform.position - contact.otherCollider.gameObject.transform.position;

                if (contact.otherCollider.gameObject.tag == "Car")
                    contact.otherCollider.gameObject.GetComponent<Car>().beginCrush = true;
            }
        }
    }

    public Vector3 crashVector;
    public bool beginCrush, beginCrushAnimate;
    internal static Action OnCrushHandler;

    void Update()
    {
        if (beginCrush && !beginCrushAnimate)
        {
            StopAllCoroutines();
            beginCrushAnimate = true;
            collider.enabled = false;
            AnimateCrush();
            OnCrushHandler?.Invoke();
        }
    }

    void AnimateCrush()
    {
        rigidbody.useGravity = false;
        rigidbody.AddForce(new Vector3(crashVector.x, Random.Range(2f, 5f), crashVector.z));
        Invoke("StopAnimation", 1f);
    }


    public void StopAnimation()
    {
        StopAllCoroutines();
        rigidbody.useGravity = true;
        rigidbody.mass = 10f;
        collider.enabled = true;
    }

    [Button]
    public void Choose()
    {
        WayPointsManager.inst.DeleteRoute(this);

        switch (carType)
        {
            case carType.big:
                Paintable.inst.ChooseOrangeBrush(GameManager.inst.keyCars[carType]);
                break;
            case carType.middle:
                Paintable.inst.ChooseBlueBrush(GameManager.inst.keyCars[carType]);
                break;
            case carType.small:
                Paintable.inst.ChooseGreenBrush(GameManager.inst.keyCars[carType]);
                break;
        }

        animation.DOPlay();
        Invoke("Release", animation.duration);
    }

    [Button]
    void Release()
    {
        animation.DOPlayBackwards();
    }
}

