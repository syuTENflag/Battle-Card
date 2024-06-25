using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    public CardView view; // �J�[�h�̌����ڂ̏���
    public CardModel model; // �J�[�h�̃f�[�^������
    public CardMovement movement;  // �ړ�(movement)�Ɋւ��邱�Ƃ𑀍�

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool playerCard) // �J�[�h�𐶐��������ɌĂ΂��֐�
    {
        model = new CardModel(cardID, playerCard); // �J�[�h�f�[�^�𐶐�
        view.Show(model); // �\��
    }

    public void DestroyCard(CardController card)
    {
        Destroy(card.gameObject);
    }

    public void DropField()
    {
        GameManager.instance.ReduceManaPoint(model.cost);
        model.FieldCard = true; // �t�B�[���h�̃J�[�h�̃t���O�𗧂Ă�
        model.canUse = false;
        view.SetCanUsePanel(model.canUse); // �o��������CanUsePanel������

        StartCoroutine(activateAbility()); // StartCoroutine�֕ύX
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

    // �J�[�h�Q�[�����ʏ����̃��\�b�h
    public IEnumerator activateAbility() // IEnumerator�^�ɕύX
    {
        // �J�[�h�g�p���̃��[�V���������s����
        yield return StartCoroutine(startCardMotion());
        
        // �X�s�[�h�A�^�b�J�[�Ȃ�U���\�ɂ���
        if (model.isSpeedAttacker)
        {
            model.canAttack = true;
            view.SetCanAttackPanel(true);
        }

        // �h���[���閇����1���ȏ�Ȃ�A���̉񐔕��J�[�h������
        if (model.drawCardNum >= 1)
        {
            for (int i = 0; i < model.drawCardNum; i++)
            {
                if (GameManager.instance.isPlayerTurn == true) // �����ǉ�
                {
                    Transform playerHand = GameObject.Find("PlayerHand").GetComponent<Transform>();
                    GameManager.instance.DrawCard(playerHand);
                }
            }
        }

        // ��D�ɉ�����g�[�N���J�[�h�̃��X�g��1�ȏ�Ȃ�A���̃��X�g��1�Ԗڂ��珇�ԂɎ�D�ɐ�������
        if (model.addCardsList.Length >= 1) // ���X�g�̑��݃`�F�b�N
        {
            for (int i = 0; i < model.addCardsList.Length; i++) // ���X�g�̌����������J��Ԃ�
            {
                if (GameManager.instance.isPlayerTurn == true)
                {
                    Transform playerHand = GameObject.Find("PlayerHand").GetComponent<Transform>();
                    GameManager.instance.CreateCard(model.addCardsList[i], playerHand);
                }
            }
        }



        // ��������g�[�N���J�[�h�̃��X�g��1�ȏ�Ȃ�A���̃��X�g��1�Ԗڂ��珇�ԂɃt�B�[���h�ɐ�������
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

        // �������[�_�[�̃��C�t��ω�
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
                //enemyImage.DOColor(new Color(1f, 0, 0), 0.5f);               //�U����H�炤�ƐԂɂȂ�
                //enemyImage.DOColor(new Color(1f, 1f, 1f), 0.5f).SetDelay(1); //�x��Ĕ��ɖ߂�

            }
            else
            {
                GameManager.instance.enemyLeaderHP += model.chgMyLeaderHpNum;
                GameManager.instance.ShowLeaderHP();
            }
        }

        // ���胊�[�_�[�̃��C�t��ω�
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

        // �}�i�|�C���g�𑝉�
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

        GameManager.instance.SetCanUsePanelHand(true); // ���ʏ����̍Ō�Ɏ��{        
    }
}