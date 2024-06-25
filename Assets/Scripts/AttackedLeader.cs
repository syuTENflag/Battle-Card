using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedLeader : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /// 攻撃
        // attackerを選択　マウスポインターに重なったカードをアタッカーにする
        CardController attackCard = eventData.pointerDrag.GetComponent<CardController>();

        GameManager.instance.AttackToLeader(attackCard, true);
    }
}