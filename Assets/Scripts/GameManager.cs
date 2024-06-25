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

    public int playerManaPoint; // �g�p����ƌ���}�i�|�C���g
    public int playerDefaultManaPoint; // ���^�[�������Ă����x�[�X�̃}�i�|�C���g
    public bool isPlayerTurn = true; //�@Public�֕ύX
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

    void StartGame() // �����l�̐ݒ� 
    {
        enemyLeaderHP = 10000;
        playerLeaderHP = 10000;
        ShowLeaderHP();

        /// �}�i�̏����l�ݒ� ///
        playerManaPoint = 1;
        playerDefaultManaPoint = 1;
        ShowManaPoint();

        // �f�b�L���V���b�t��
        ShuffleDeck();

        // ������D��z��
        SetStartHand();

        // �^�[���̌���
        StartCoroutine(TurnCalc());
    }

    public void ShuffleDeck() // �f�b�L���V���b�t�����郁�\�b�h
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
        // �v���C���[�̃^�[���łȂ���Ώ��������𒆒f����
        if (!isPlayerTurn && place == playerHand)
        {
            return;
        }

        CardController card = Instantiate(cardPrefab, place);

        // Player�̎�D�ɐ������ꂽ�J�[�h��Player�̃J�[�h�Ƃ���
        if (place == playerHand)
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }

    public IEnumerator SummonCard(int cardID, bool isPlayer) // IEnumerator�^�֕ύX
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

    public void DrawCard(Transform hand) // �J�[�h������
    {
        // �f�b�L���Ȃ��Ȃ�����Ȃ�
        if (deck.Count == 0)
        {
            return;
        }

        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();

        if (playerHandCardList.Length < 9)
        {
            // �f�b�L�̈�ԏ�̃J�[�h�𔲂����A��D�ɉ�����
            int cardID = deck[0];
            deck.RemoveAt(0);
            CreateCard(cardID, hand);
            //SoundManager.instance.PlaySE(1); // pra7 
        }

        SetCanUsePanelHand(true);
    }

    void SetStartHand() // ��D��3���z��
    {
        for (int i = 0; i < 3; i++)
        {
            DrawCard(playerHand);
        }
    }

    IEnumerator TurnCalc() // �^�[�����Ǘ�����
    {
        yield return StartCoroutine(uIManager.ShowChangeTurnPanel());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            //EnemyTurn(); 
            StartCoroutine(EnemyTurn()); // StartCoroutine�ŌĂяo��
        }
    }

    public void ChangeTurn() // �^�[���G���h�{�^���ɂ��鏈��
    {
        if (isPlayerTurn)
        {
            CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
            SetAttackableFieldCard(playerFieldCardList, false); // Player�̏�̃J�[�h���U���s�ɂ���
            SetCanUsePanelHand(false); // ��D�̃J�[�h���g�p�s�ɂ���

            SoundManager.instance.PlaySE(0);
        }
        else
        {
            CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
            SetAttackableFieldCard(enemyFieldCardList, false); // Enemy�̏�̃J�[�h���U���s�ɂ���
        }

        // �^�[���G���h�{�^���������\/�s�\�ɂ���
        turnEndButton.interactable = !turnEndButton.interactable;

        isPlayerTurn = !isPlayerTurn; // �^�[�����t�ɂ���

        if (isPlayerTurn)
        {
            SetCanUsePanelHand(true); // Player�̎�D���g�p�\�ɂ���
        }

        StartCoroutine(TurnCalc()); // �^�[���𑊎�ɉ�
    }

    void PlayerTurn()
    {
        Debug.Log("Player�̃^�[��");

        CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(playerFieldCardList, true);

        /// �}�i�𑝂₷
        playerDefaultManaPoint++;
        playerManaPoint = playerDefaultManaPoint;
        ShowManaPoint();

        DrawCard(playerHand); // ��D���ꖇ������
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy�̃^�[��");

        CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();

        yield return new WaitForSeconds(1f);

        SetAttackableFieldCard(enemyFieldCardList, true); // �G�̃t�B�[���h�̃J�[�h���U���\�ɂ���

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

        ChangeTurn(); // �^�[���G���h����
    }

    public void CardBattle(CardController attackCard, CardController defenceCard)
    {
        // �U���J�[�h�ƍU�������J�[�h�������v���C���[�̃J�[�h�Ȃ�o�g�����Ȃ�
        if (attackCard.model.PlayerCard == defenceCard.model.PlayerCard)
        {
            return;
        }

        // �U���J�[�h���A�^�b�N�\�łȂ���΍U�����Ȃ��ŏ����I������
        if (attackCard.model.canAttack == false)
        {
            return;
        }

        if (attackCard.model.PlayerCard)
        {
            SoundManager.instance.PlaySE(1); // �U�����̉���炷
        }

        // �U�����ꂽ���̃p���[�����������ꍇ�A�U�����̃J�[�h��j�󂷂�
        if (attackCard.model.power < defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
        }
        // �p���[�������������ꍇ�A�����̃J�[�h��j�󂷂�
        else if (attackCard.model.power == defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
            defenceCard.DestroyCard(defenceCard);
        }
        // �U�����̃p���[�������ꍇ�A�h�䑤�̃J�[�h��j�󂷂�
        else
        {
            defenceCard.DestroyCard(defenceCard);
        }

        // �U���J�[�h�̍U���t���O��false�ɂ���
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
            return; // �����𒆒f�����ɐi�s����
        }

        if (attackCard.model.PlayerCard == true) // attackCard���v���C���[�̃J�[�h�Ȃ�
        {
            SoundManager.instance.PlaySE(1); // �U�����̉���炷
            enemyLeaderHP -= attackCard.model.power; // �G�̃��[�_�[��HP�����炷

            enemyImage.DOColor(new Color(1f, 0, 0), 0.5f);               //�U����H�炤�ƐԂɂȂ�
            enemyImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //�x��Ĕ��ɖ߂�
        }
        else // attackCard���G�̃J�[�h�Ȃ�
        {
            playerLeaderHP -= attackCard.model.power; // �v���C���[�̃��[�_�[��HP�����炷

            playerImage.DOColor(new Color(1f, 0, 0), 0.5f);               //�U����H�炤�ƐԂɂȂ�
            playerImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //�x��Ĕ��ɖ߂�
        }

        //enemyLeaderHP -= attackCard.model.power; // �R�����g�A�E�g����

        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
        Debug.Log("�G��HP�́A" + enemyLeaderHP);
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

    public void ShowManaPoint() // �}�i�|�C���g��\�����郁�\�b�h
    {
        playerManaPointText.text = playerManaPoint.ToString();
        playerDefaultManaPointText.text = playerDefaultManaPoint.ToString();
    }

    public void ReduceManaPoint(int cost) // �R�X�g�̕��A�}�i�|�C���g�����炷
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
                if (card.model.cost <= playerManaPoint && isPlayerTurn) // �v���C���[�̃^�[�����ǂ������`�F�b�N
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
        foreach (Transform n in playerHand.transform)�@// Player�̎�D�̃J�[�h��S�Ĕj�󂷂�
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in playerField.transform)�@// Player�̃t�B�[���h�̃J�[�h��S�Ĕj�󂷂�
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in enemyField.transform)�@// Enemy�̃t�B�[���h�̃J�[�h��S�Ĕj�󂷂�
        {
            GameObject.Destroy(n.gameObject);
        }
        isPlayerTurn = true; // �G�^�[���������ꍇ�ɁA���X�^�[�g��ɓG�^�[������n�܂��Ă��܂���
        deck = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 1, 3, 4, 2, 3, 1, 2, 5, 6 }; // �f�b�L���X�g�̐ݒ�
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
        // �Q�[���I�����̏������L�q
        Debug.Log("�Q�[���I��");
        StartGame();
    }
}
