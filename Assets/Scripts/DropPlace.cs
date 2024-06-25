using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// フィールドにアタッチするクラス
public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) // ドロップされた時に行う処理
    {
        //CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>(); // ドラッグしてきた情報からCardMovementを取得
        CardController card = eventData.pointerDrag.GetComponent<CardController>(); // 今回の書き換え部分

        if (card != null) // もしカードがあれば、
        {
            if (card.model.canUse == true)  // 使用可能なカードなら、
            {
                card.movement.cardParent = this.transform; // カードの親要素を自分（アタッチされてるオブジェクト）にする 今回の書き換え部分
                card.DropField(); // カードをフィールドに置いた時の処理を行う
            }
        }
    }
}