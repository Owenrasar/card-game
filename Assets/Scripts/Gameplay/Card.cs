using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    [SerializeReference] public List<Action> actions = new List<Action>();
    public int cost;
    public string name;

}