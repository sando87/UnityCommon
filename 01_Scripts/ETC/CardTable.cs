using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTable", menuName = "Scriptable Object Asset/CardTable")]
public class CardTable : ScriptableObjectTable<CardInfo>
{

}

[System.Serializable]
public class CardInfo
{
}
