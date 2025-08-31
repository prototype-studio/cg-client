using Core.Match;
using UnityEngine;

public class Character : MonoBehaviour, ICharacter
{
    [SerializeField] private CharacterData data;
    
    //PROPERTIES
    public CharacterData Data => data;
}