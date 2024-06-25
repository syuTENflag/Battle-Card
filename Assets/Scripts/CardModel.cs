using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class CardModel
{
    public int cardId;
    public string name;
    public int cost;
    public int power;
    public Sprite icon;
    public CardType cardType;
    public string description;

    public bool isSpeedAttacker;
    public int drawCardNum;
    public int[] addCardsList;
    public int[] summonCardsList;
    public int chgMyLeaderHpNum;
    public int chgEnemyLeaderHpNum;
    public int manaBoostNum;

    public bool canUse = false;
    public bool PlayerCard = false;
    public bool FieldCard = false;
    public bool canAttack = false;

    public CardModel(int cardID, bool playerCard) // データを受け取り、その処理
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card" + cardID); // CardEntityのパス

        cardID = cardEntity.cardID;
        name = cardEntity.name;
        cost = cardEntity.cost;
        power = cardEntity.power;
        icon = cardEntity.icon;

        cardType = cardEntity.cardType;
        description = cardEntity.description;

        isSpeedAttacker = cardEntity.isSpeedAttacker;
        drawCardNum = cardEntity.drawCardNum;
        addCardsList = cardEntity.addCardsList;
        summonCardsList = cardEntity.summonCardsList;
        chgMyLeaderHpNum = cardEntity.chgMyLeaderHpNum;
        chgEnemyLeaderHpNum = cardEntity.chgEnemyLeaderHpNum;
        manaBoostNum = cardEntity.manaBoostNum;

        PlayerCard = playerCard;
    }
}