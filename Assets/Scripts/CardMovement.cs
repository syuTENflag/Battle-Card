using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform cardParent; // 現在のカードの親オブジェクト
    bool canDrag = true; // カードを動かせるかどうかのフラグ

    Transform oriCardParent; // ドラッグ開始時の親オブジェクト
    int cardIndex; // ドラッグ開始時のカードの順番

    // ドラッグを始めるときに行う処理
    public void OnBeginDrag(PointerEventData eventData)
    {
        CardController card = GetComponent<CardController>(); // カードコントローラーを取得
        canDrag = true;

        // プレイヤーのカードでなければドラッグ不可
        if (!card.model.PlayerCard)
        {
            canDrag = false;
        }

        // 手札のカードの場合
        if (card.model.FieldCard == false)
        {
            // 使用可能でない場合ドラッグ不可
            if (card.model.canUse == false)
            {
                canDrag = false;
            }
        }
        else // フィールドのカードの場合
        {
            // 攻撃可能でない場合ドラッグ不可
            if (card.model.canAttack == false)
            {
                canDrag = false;
            }
        }

        // ドラッグ不可なら処理を終了
        if (!canDrag)
        {
            return;
        }

        // 親オブジェクトとインデックスを保存
        cardParent = transform.parent;
        oriCardParent = transform.parent;
        cardIndex = this.transform.GetSiblingIndex();

        // 親オブジェクトを変更してレイキャストを無効化
        transform.SetParent(cardParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ不可なら処理を終了
        if (!canDrag)
        {
            return;
        }

        // カードの位置を更新
        transform.position = eventData.position;
    }

    // ドラッグを終了したときの処理
    public void OnEndDrag(PointerEventData eventData)
    {
        // ドラッグ不可なら処理を終了
        if (!canDrag)
        {
            return;
        }

        // 親オブジェクトを元に戻す
        transform.SetParent(cardParent, false);

        // 元の親と同じなら元のインデックスに戻す
        if (oriCardParent == cardParent)
        {
            transform.SetSiblingIndex(cardIndex);
        }

        // レイキャストを有効化
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    // 攻撃モーションのコルーチン
    public IEnumerator AttackMotion(Transform target)
    {
        Vector3 currentPosition = transform.position; // 現在の位置を保存
        cardParent = transform.parent;
        int attackCardIndex = transform.GetSiblingIndex(); // 子要素の順番情報を取得

        // 一時的に親オブジェクトを変更
        transform.SetParent(cardParent.parent);

        // ターゲットに向かって移動
        transform.DOMove(target.position, 0.75f).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(0.75f);

        // 攻撃時の音を鳴らす
        SoundManager.instance.PlaySE(1);

        // 元の位置に戻る
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);

        // 親オブジェクトを元に戻し、インデックスを設定
        transform.SetParent(cardParent);
        transform.SetSiblingIndex(attackCardIndex);
    }

    // 召喚モーションのコルーチン
    public IEnumerator SummonMotion()
    {
        // 召喚時の音を出す
        SoundManager.instance.PlaySE(2);

        // 0.5秒かけて1.2倍の大きさにする
        transform.DOScale(1.2f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 0.5秒かけて元の大きさにする
        transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);
    }

    // スペル使用モーションのコルーチン
    public IEnumerator UseSpellMotion()
    {
        // スペル使用時の音を出す
        SoundManager.instance.PlaySE(3);

        yield return new WaitForSeconds(0.1f);

        // 一時的に親オブジェクトを変更
        transform.SetParent(transform.root);

        // 0.5秒かけて3倍の大きさにする
        transform.DOScale(3.0f, 0.5f);
        yield return new WaitForSeconds(0.7f);

        // 0.3秒かけて5倍の大きさにする
        transform.DOScale(5.0f, 0.3f);
        SoundManager.instance.PlaySE(4);
        yield return new WaitForSeconds(0.3f);

        // 0秒でサイズを0にする
        transform.DOScale(0.0f, 0.0f);
        yield return new WaitForSeconds(0.0f);
    }
}
