using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTable", menuName = "Scriptable Object Asset/CardTable")]
public class CardTable : ScriptableObjectTable<CardInfo>
{

}

[System.Serializable]
public class CardInfo
{
    [SerializeField] private int id = 0;
    [SerializeField] private string name = "";
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private Sprite image = null;

    public int ID => id;
    public string Name => name;
}
