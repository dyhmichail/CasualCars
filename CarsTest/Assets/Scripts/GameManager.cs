using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum carType
{
    middle,
    small,
    big
};
public class GameManager : SerializedMonoBehaviour
{
    public Dictionary<carType, Car> keyCars;

    [SerializeField] private int carsArrived;
    public void CarsArrived()
    {
        Debug.Log("carsArrived");
        carsArrived++;
        if (carsArrived == 3)
            WinGame();
    }

    [SerializeField] private GameObject winTitles, loseTitles;
    public void WinGame()
    {
        winTitles.SetActive(true);
    }

    public void LoseGame()
    {
        loseTitles.SetActive(true);
    }

    //создаем синглтон
    public static GameManager inst;
    void Awake()
    {
        inst = this;
        Car.OnCrushHandler += LoseGame;
    }

    void OnDestroy()
    {
        Car.OnCrushHandler -= LoseGame;
    }


    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
