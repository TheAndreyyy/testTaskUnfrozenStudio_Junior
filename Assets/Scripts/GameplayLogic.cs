using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLogic : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField]
    int turnCount = 1;
    [SerializeField]
    GameObject characterPrefab;
    [SerializeField]
    GameObject roundPrefab;
    [SerializeField]
    Transform content;
    public SpawnerScriptableArmys redArmy;
    public SpawnerScriptableArmys blueArmy;
    public int actionsPerRound;
    public int actionsDone = 0;
    public int currentRound = 1;
    public List<SpawnerScriptableArmys.characterClass> generalTurn = new List<SpawnerScriptableArmys.characterClass>();

    void Start()
    {
        actionsPerRound = blueArmy.armyCharacters.Count + redArmy.armyCharacters.Count;
        actionsDone = 0;
        FillUpGeneralTurn();
        SortingToRightTurn();
        GenerateTurn();
    }

    void FillUpGeneralTurn()
    {
        for (int i = 0; i < blueArmy.armyCharacters.Count; i++)
        {
            generalTurn.Add(blueArmy.armyCharacters[i]);
        }
        for (int i = 0; i < redArmy.armyCharacters.Count; i++)
        {
            generalTurn.Add(redArmy.armyCharacters[i]);
        }
    }//заполняем главный список "живых" персонажей из двух армий

    void SortingToRightTurn()
    {
        for (int i = 0; i < generalTurn.Count - 1; i++)
        {
            for (int j = 0; j < generalTurn.Count - 1; j++)
            {
                if (generalTurn[j].initiativeValue <= generalTurn[j + 1].initiativeValue)
                {
                    if (generalTurn[j].initiativeValue < generalTurn[j + 1].initiativeValue)
                    {
                        Bubble(j, j + 1);
                    }
                    else//init ==
                    {
                        if (generalTurn[j].speedValue <= generalTurn[j + 1].speedValue)
                        {
                            if (generalTurn[j].speedValue < generalTurn[j + 1].speedValue)
                            {
                                Bubble(j, j + 1);
                            }
                            else//speed ==
                            {
                                if (generalTurn[j].color == generalTurn[j + 1].color)//color ==
                                {
                                    if (generalTurn[j].cellNumber > generalTurn[j + 1].cellNumber)
                                    {
                                        Bubble(j, j + 1);
                                    }
                                }
                                else
                                {
                                    if (currentRound % 2 == 0)
                                    {
                                        if (generalTurn[j].color == SpawnerScriptableArmys.enumArmyColor.red)
                                        {
                                            Bubble(j, j + 1);
                                        }
                                    }
                                    else
                                    {
                                        if (generalTurn[j].color == SpawnerScriptableArmys.enumArmyColor.blue)
                                        {
                                            Bubble(j, j + 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }//сортируем главный список согласно правилам

    void Bubble(int a, int b)
    {
        SpawnerScriptableArmys.characterClass bubble = generalTurn[a];
        generalTurn[a] = generalTurn[b];
        generalTurn[b] = bubble;
    }//простейшая сортировка

    void GenerateTurn()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        int spawnedRounds = 0;
        int offset = actionsDone + 0;
        for (int i = 0; i < turnCount; i++)
        {
            SpawnCell(offset, i + 1);
            Debug.LogError($"index {i} cell {offset++}");
            Debug.LogError($"generalTurn {generalTurn.Count} | index {i} | offset {offset}");
            //if (offset % generalTurn.Count == 0)
            if (offset >= generalTurn.Count)
            {
                offset = 0;
                spawnedRounds++;
                Debug.LogError($"round {spawnedRounds}");
                SpawnRound();
            }
        }

        void SpawnCell(int offset, int index)
        {
            GameObject generatedCharacter = Instantiate(characterPrefab, content);
            CellLogic generatedCharacterLogic = generatedCharacter.GetComponent<CellLogic>();
            FillInfo(generalTurn[offset], generatedCharacterLogic, index);
        }//спавн персонажа

        void SpawnRound()
        {
            GameObject generatedRound = Instantiate(roundPrefab, content);
            CellLogic generatedRoundLogic = generatedRound.GetComponent<CellLogic>();
            generatedRoundLogic.colorOfBG.color = Color.white;
            spawnedRounds++;
            generatedRoundLogic.roundValue.text = Convert.ToString(spawnedRounds);
        }//спавн ячейки "раунда"
    }//спавним главный список в виде объектов

    void FillInfo(SpawnerScriptableArmys.characterClass characterInfo, CellLogic logicInfo, int index)
    {
        logicInfo.turnPosition.text = Convert.ToString(index);
        logicInfo.initiativeValue.text = Convert.ToString(characterInfo.initiativeValue);
        logicInfo.speedValue.text = Convert.ToString(characterInfo.speedValue);
        if (characterInfo.color == SpawnerScriptableArmys.enumArmyColor.blue)
        {
            logicInfo.cellNumber.text = "С" + Convert.ToString(characterInfo.cellNumber) + ":";
            logicInfo.colorOfBG.color = new Color32(45, 155, 240, 255);//blue color
        }
        else
        {
            logicInfo.cellNumber.text = "К" + Convert.ToString(characterInfo.cellNumber) + ":";
            logicInfo.colorOfBG.color = new Color32(242, 71, 38, 255);//red color
        }
    }//заполняем информацию объекта из главного списка

    public void NextTurn()
    {
        SpawnerScriptableArmys.characterClass temp = generalTurn[0];
        generalTurn.RemoveAt(0);
        generalTurn.Add(temp);
        GenerateTurn();
        actionsDone++;
    }//метод для кнопки "пропустить ход"

    public void KillNextCharacter()//метод для кнопки "убить следующего воина в очереди"
    {
        if (generalTurn[0].color != generalTurn[1].color)
        {
            if (actionsPerRound > 1)
            {
                foreach (Transform child in content.transform)
                {
                    Destroy(child.gameObject);
                }
                SpawnerScriptableArmys.characterClass temp = generalTurn[0];
                generalTurn.RemoveAt(0);
                generalTurn.RemoveAt(0);
                actionsPerRound -= 1;
                generalTurn.Add(temp);
                GenerateTurn();
                actionsDone++;
            }
            else
            {
                Debug.LogError($"Некого убивать, все убиты");
            }
        }
        else
        {
            Debug.LogError($"Нельзя убивать своих, пропустите ход");
        }

    }
}//was 284 strings
