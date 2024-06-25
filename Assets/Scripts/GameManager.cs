using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIManager uIManager;

    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform playerHand, playerField, enemyField;
    [SerializeField] Text playerLeaderHPText;
    [SerializeField] Text enemyLeaderHPText;
    [SerializeField] Text playerManaPointText;
    [SerializeField] Text playerDefaultManaPointText;
    [SerializeField] Transform playerLeaderTransform;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Image playerImage;

    [SerializeField] Button turnEndButton;

    public int playerManaPoint; // 使用すると減るマナポイント
    public int playerDefaultManaPoint; // 毎ターン増えていくベースのマナポイント
    public bool isPlayerTurn = true; //　Publicへ変更
    List<int> deck = new List<int>() { 1, 2, 8, 1, 4, 8, 5, 6, 3, 7, 1, 3, 4, 5, 7, 3, 2, 3 };  //

    public Image PlayerImage
    {
        get { return playerImage; }
        private set { playerImage = value; }
    }

    public Image EnemyImage
    {
        get { return enemyImage; }
        private set { enemyImage = value; }
    }

    public static GameManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame() // 初期値の設定 
    {
        enemyLeaderHP = 10000;
        playerLeaderHP = 10000;
        ShowLeaderHP();

        /// マナの初期値設定 ///
        playerManaPoint = 1;
        playerDefaultManaPoint = 1;
        ShowManaPoint();

        // デッキをシャッフル
        ShuffleDeck();

        // 初期手札を配る
        SetStartHand();

        // ターンの決定
        StartCoroutine(TurnCalc());
    }

    public void ShuffleDeck() // デッキをシャッフルするメソッド
    {
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public void CreateCard(int cardID, Transform place)
    {
        // プレイヤーのターンでなければ召喚処理を中断する
        if (!isPlayerTurn && place == playerHand)
        {
            return;
        }

        CardController card = Instantiate(cardPrefab, place);

        // Playerの手札に生成されたカードはPlayerのカードとする
        if (place == playerHand)
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }

    public IEnumerator SummonCard(int cardID, bool isPlayer) // IEnumerator型へ変更
    {
        if (isPlayer == true)
        {
            CardController card = Instantiate(cardPrefab, playerField);
            card.Init(cardID, true);
            card.model.FieldCard = true;
            yield return StartCoroutine(card.activateAbility());
        }
        else
        {
            CardController card = Instantiate(cardPrefab, enemyField);
            card.Init(cardID, false);
            card.model.FieldCard = true;
            yield return StartCoroutine(card.activateAbility());
        }
    }

    public void DrawCard(Transform hand) // カードを引く
    {
        // デッキがないなら引かない
        if (deck.Count == 0)
        {
            return;
        }

        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();

        if (playerHandCardList.Length < 9)
        {
            // デッキの一番上のカードを抜き取り、手札に加える
            int cardID = deck[0];
            deck.RemoveAt(0);
            CreateCard(cardID, hand);
            //SoundManager.instance.PlaySE(1); // pra7 
        }

        SetCanUsePanelHand(true);
    }

    void SetStartHand() // 手札を3枚配る
    {
        for (int i = 0; i < 3; i++)
        {
            DrawCard(playerHand);
        }
    }

    IEnumerator TurnCalc() // ターンを管理する
    {
        yield return StartCoroutine(uIManager.ShowChangeTurnPanel());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            //EnemyTurn(); 
            StartCoroutine(EnemyTurn()); // StartCoroutineで呼び出す
        }
    }

    public void ChangeTurn() // ターンエンドボタンにつける処理
    {
        if (isPlayerTurn)
        {
            CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
            SetAttackableFieldCard(playerFieldCardList, false); // Playerの場のカードを攻撃不可にする
            SetCanUsePanelHand(false); // 手札のカードを使用不可にする

            SoundManager.instance.PlaySE(0);
        }
        else
        {
            CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
            SetAttackableFieldCard(enemyFieldCardList, false); // Enemyの場のカードを攻撃不可にする
        }

        // ターンエンドボタンを押下可能/不可能にする
        turnEndButton.interactable = !turnEndButton.interactable;

        isPlayerTurn = !isPlayerTurn; // ターンを逆にする

        if (isPlayerTurn)
        {
            SetCanUsePanelHand(true); // Playerの手札を使用可能にする
        }

        StartCoroutine(TurnCalc()); // ターンを相手に回す
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");

        CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(playerFieldCardList, true);

        /// マナを増やす
        playerDefaultManaPoint++;
        playerManaPoint = playerDefaultManaPoint;
        ShowManaPoint();

        DrawCard(playerHand); // 手札を一枚加える
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");

        CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();

        yield return new WaitForSeconds(1f);

        SetAttackableFieldCard(enemyFieldCardList, true); // 敵のフィールドのカードを攻撃可能にする

        yield return new WaitForSeconds(1f);

        if (enemyFieldCardList.Length < 5)
        {
            if (enemyFieldCardList.Length < 2)
            {
                yield return StartCoroutine(SummonCard(1, false));
                yield return StartCoroutine(SummonCard(2, false));
            }
            else
            {
                yield return StartCoroutine(SummonCard(3, false));
            }
        }

        CardController[] enemyFieldCardListSecond = enemyField.GetComponentsInChildren<CardController>();

        yield return new WaitForSeconds(1f);

        while (Array.Exists(enemyFieldCardListSecond, card => card.model.canAttack))
        {
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyFieldCardListSecond, card => card.model.canAttack);
            CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();

            CardController attackCard = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                int defenceCardNumber = UnityEngine.Random.Range(0, playerFieldCardList.Length);
                CardController defenceCard = playerFieldCardList[defenceCardNumber];
                yield return StartCoroutine(attackCard.movement.AttackMotion(defenceCard.transform));
                CardBattle(attackCard, defenceCard);
            }
            else
            {
                yield return StartCoroutine(attackCard.movement.AttackMotion(playerLeaderTransform));
                AttackToLeader(attackCard, false);
            }

            yield return new WaitForSeconds(1f);

            enemyFieldCardListSecond = enemyField.GetComponentsInChildren<CardController>();
        }

        ChangeTurn(); // ターンエンドする
    }

    public void CardBattle(CardController attackCard, CardController defenceCard)
    {
        // 攻撃カードと攻撃されるカードが同じプレイヤーのカードならバトルしない
        if (attackCard.model.PlayerCard == defenceCard.model.PlayerCard)
        {
            return;
        }

        // 攻撃カードがアタック可能でなければ攻撃しないで処理終了する
        if (attackCard.model.canAttack == false)
        {
            return;
        }

        if (attackCard.model.PlayerCard)
        {
            SoundManager.instance.PlaySE(1); // 攻撃時の音を鳴らす
        }

        // 攻撃された側のパワーが高かった場合、攻撃側のカードを破壊する
        if (attackCard.model.power < defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
        }
        // パワーが同じだった場合、両方のカードを破壊する
        else if (attackCard.model.power == defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
            defenceCard.DestroyCard(defenceCard);
        }
        // 攻撃側のパワーが高い場合、防御側のカードを破壊する
        else
        {
            defenceCard.DestroyCard(defenceCard);
        }

        // 攻撃カードの攻撃フラグをfalseにする
        attackCard.model.canAttack = false;
    }

    void SetAttackableFieldCard(CardController[] cardList, bool canAttack)
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttack = canAttack;
            card.view.SetCanAttackPanel(canAttack);
        }
    }

    public int playerLeaderHP;
    public int enemyLeaderHP;

    public void AttackToLeader(CardController attackCard, bool isPlayerCard)
    {
        if (attackCard.model.canAttack == false)
        {
            return; // 処理を中断せずに進行する
        }

        if (attackCard.model.PlayerCard == true) // attackCardがプレイヤーのカードなら
        {
            SoundManager.instance.PlaySE(1); // 攻撃時の音を鳴らす
            enemyLeaderHP -= attackCard.model.power; // 敵のリーダーのHPを減らす

            enemyImage.DOColor(new Color(1f, 0, 0), 0.5f);               //攻撃を食らうと赤になる
            enemyImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //遅れて白に戻る
        }
        else // attackCardが敵のカードなら
        {
            playerLeaderHP -= attackCard.model.power; // プレイヤーのリーダーのHPを減らす

            playerImage.DOColor(new Color(1f, 0, 0), 0.5f);               //攻撃を食らうと赤になる
            playerImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //遅れて白に戻る
        }

        //enemyLeaderHP -= attackCard.model.power; // コメントアウトする

        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
        Debug.Log("敵のHPは、" + enemyLeaderHP);
        ShowLeaderHP();
    }

    public void ShowLeaderHP()
    {
        if (playerLeaderHP <= 0)
        {
            playerLeaderHP = 0;
            uIManager.ShowGameEndPanel(false);
            StopAllCoroutines();
        }
        if (enemyLeaderHP <= 0)
        {
            enemyLeaderHP = 0;
            uIManager.ShowGameEndPanel(true);
            StopAllCoroutines();
        }

        playerLeaderHPText.text = playerLeaderHP.ToString();
        enemyLeaderHPText.text = enemyLeaderHP.ToString();
    }

    public void ShowManaPoint() // マナポイントを表示するメソッド
    {
        playerManaPointText.text = playerManaPoint.ToString();
        playerDefaultManaPointText.text = playerDefaultManaPoint.ToString();
    }

    public void ReduceManaPoint(int cost) // コストの分、マナポイントを減らす
    {
        playerManaPoint -= cost;
        ShowManaPoint();

        if (isPlayerTurn)
        {
            SetCanUsePanelHand(true);
        }
    }

    public void SetCanUsePanelHand(bool isAttachPanel)
    {
        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();

        if (isAttachPanel)
        {
            foreach (CardController card in playerHandCardList)
            {
                if (card.model.cost <= playerManaPoint && isPlayerTurn) // プレイヤーのターンかどうかをチェック
                {
                    card.model.canUse = true;
                    card.view.SetCanUsePanel(card.model.canUse);
                }
                else
                {
                    card.model.canUse = false;
                    card.view.SetCanUsePanel(card.model.canUse);
                }
            }
        }
        else
        {
            foreach (CardController card in playerHandCardList)
            {
                card.model.canUse = false;
                card.view.SetCanUsePanel(card.model.canUse);
            }
        }
    }

    public void RestartGame()
    {
        foreach (Transform n in playerHand.transform)　// Playerの手札のカードを全て破壊する
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in playerField.transform)　// Playerのフィールドのカードを全て破壊する
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in enemyField.transform)　// Enemyのフィールドのカードを全て破壊する
        {
            GameObject.Destroy(n.gameObject);
        }
        isPlayerTurn = true; // 敵ターンだった場合に、リスタート後に敵ターンから始まってしまう為
        deck = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 1, 3, 4, 2, 3, 1, 2, 5, 6 }; // デッキリストの設定
        uIManager.HideGameEndPanel();
        StartGame();
    }

    void CheckGameEnd()
    {
        if (enemyLeaderHP <= 0 || playerLeaderHP <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        // ゲーム終了時の処理を記述
        Debug.Log("ゲーム終了");
        StartGame();
    }
}
