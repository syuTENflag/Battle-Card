using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    public CardView view; // カードの見た目の処理
    public CardModel model; // カードのデータを処理
    public CardMovement movement;  // 移動(movement)に関することを操作

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool playerCard) // カードを生成した時に呼ばれる関数
    {
        model = new CardModel(cardID, playerCard); // カードデータを生成
        view.Show(model); // 表示
    }

    public void DestroyCard(CardController card)
    {
        Destroy(card.gameObject);
    }

    public void DropField()
    {
        GameManager.instance.ReduceManaPoint(model.cost);
        model.FieldCard = true; // フィールドのカードのフラグを立てる
        model.canUse = false;
        view.SetCanUsePanel(model.canUse); // 出した時にCanUsePanelを消す

        StartCoroutine(activateAbility()); // StartCoroutineへ変更
    }

    public IEnumerator startCardMotion()
    {
        switch (model.cardType)
        {
            case CardType.Monster:
                yield return StartCoroutine(movement.SummonMotion());
                break;

            case CardType.Spell:
                yield return StartCoroutine(movement.UseSpellMotion());
                break;
        }
    }

    // カードゲーム効果処理のメソッド
    public IEnumerator activateAbility() // IEnumerator型に変更
    {
        // カード使用時のモーションを実行する
        yield return StartCoroutine(startCardMotion());
        
        // スピードアタッカーなら攻撃可能にする
        if (model.isSpeedAttacker)
        {
            model.canAttack = true;
            view.SetCanAttackPanel(true);
        }

        // ドローする枚数が1枚以上なら、その回数分カードを引く
        if (model.drawCardNum >= 1)
        {
            for (int i = 0; i < model.drawCardNum; i++)
            {
                if (GameManager.instance.isPlayerTurn == true) // 分岐を追加
                {
                    Transform playerHand = GameObject.Find("PlayerHand").GetComponent<Transform>();
                    GameManager.instance.DrawCard(playerHand);
                }
            }
        }

        // 手札に加えるトークンカードのリストが1つ以上なら、そのリストの1番目から順番に手札に生成する
        if (model.addCardsList.Length >= 1) // リストの存在チェック
        {
            for (int i = 0; i < model.addCardsList.Length; i++) // リストの個数分処理を繰り返す
            {
                if (GameManager.instance.isPlayerTurn == true)
                {
                    Transform playerHand = GameObject.Find("PlayerHand").GetComponent<Transform>();
                    GameManager.instance.CreateCard(model.addCardsList[i], playerHand);
                }
            }
        }



        // 召喚するトークンカードのリストが1つ以上なら、そのリストの1番目から順番にフィールドに生成する
        if (model.summonCardsList.Length >= 1)
        {
            if (GameManager.instance.isPlayerTurn == true)
            {
                for (int i = 0; i < model.summonCardsList.Length; i++)
                {
                    yield return StartCoroutine(GameManager.instance.SummonCard(model.summonCardsList[i], true));
                }
            }
            else
            {
                for (int i = 0; i < model.summonCardsList.Length; i++)
                {
                    yield return StartCoroutine(GameManager.instance.SummonCard(model.summonCardsList[i], false));
                }
            }
        }

        // 自分リーダーのライフを変化
        if (model.chgMyLeaderHpNum != 0)
        {
            if (model.chgMyLeaderHpNum > 0)
            {
                SoundManager.instance.PlaySE(5);
            }
            else
            {
                SoundManager.instance.PlaySE(6);
            }

            if (GameManager.instance.isPlayerTurn == true)
            {
                GameManager.instance.playerLeaderHP += model.chgMyLeaderHpNum;
                GameManager.instance.ShowLeaderHP();
                //enemyImage.DOColor(new Color(1f, 0, 0), 0.5f);               //攻撃を食らうと赤になる
                //enemyImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //遅れて白に戻る

            }
            else
            {
                GameManager.instance.enemyLeaderHP += model.chgMyLeaderHpNum;
                GameManager.instance.ShowLeaderHP();
            }
        }

        // 相手リーダーのライフを変化
        if (model.chgEnemyLeaderHpNum != 0)
        {
            if (model.chgMyLeaderHpNum > 0)
            {
                SoundManager.instance.PlaySE(5);
            }
            else
            {
                SoundManager.instance.PlaySE(6);
            }

            if (GameManager.instance.isPlayerTurn == true)
            {
                GameManager.instance.enemyLeaderHP += model.chgEnemyLeaderHpNum;
                GameManager.instance.ShowLeaderHP();
            }
            else
            {
                GameManager.instance.playerLeaderHP += model.chgEnemyLeaderHpNum;
                GameManager.instance.ShowLeaderHP();
            }
        }

        // マナポイントを増加
        if (model.manaBoostNum >= 1)
        {
            if (GameManager.instance.isPlayerTurn == true)
            {
                GameManager.instance.playerDefaultManaPoint += model.manaBoostNum;
                GameManager.instance.ShowManaPoint();
            }
        }

        if (GameManager.instance.isPlayerTurn)
        {
            GameManager.instance.SetCanUsePanelHand(true);
        }

        if (model.cardType == CardType.Spell)
        {
            DestroyCard(this);
        }

        GameManager.instance.SetCanUsePanelHand(true); // 効果処理の最後に実施        
    }
}