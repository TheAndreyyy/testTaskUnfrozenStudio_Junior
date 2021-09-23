using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnedArmy", menuName = "ScriptableObjects/SpawnArmy", order = 1)]
public class SpawnerScriptableArmys : ScriptableObject
{
    public GameObject characterPrefab;
    public CellLogic characterLogic;
    public enum enumArmyColor
    {
        blue,
        red
    }
    public enumArmyColor armyColor;

    [System.Serializable]
    public class characterClass
    {
        public enumArmyColor color;
        public int cellNumber = 1;
        [Range(1, 9)]
        public int initiativeValue = 1;
        [Range(1, 6)]
        public int speedValue = 1;
    }
    public List<characterClass> armyCharacters;
}
