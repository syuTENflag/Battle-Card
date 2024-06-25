using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEntity", menuName = "Create CardEntity")]

public class CardEntity : ScriptableObject
{
    public int cardID;
    public new string name;
    public int cost;
    public int power;
    public Sprite icon;

    public CardType cardType; // カードタイプの選択
    [Multiline] public string description; // 効果テキストの入力箇所

    [Space(15)] // スペースを空ける
    [Header("■ 効果設定")]
    public bool isSpeedAttacker; // スピードアタッカー(疾走、召喚酔いしない)かどうか
    public int drawCardNum; // カードを引く枚数
    public int[] addCardsList; // 手札に加えるカードのリスト
    public int[] summonCardsList; // 召喚するカードのリスト
    public int chgMyLeaderHpNum; // 自分リーダーのHPを変化させる値
    public int chgEnemyLeaderHpNum; // 相手リーダーのHPを変化させる値
    public int manaBoostNum; // マナポイント(PP)の上昇値
}

public enum CardType // カードタイプの種類の定義
{
    Monster,
    Spell
}