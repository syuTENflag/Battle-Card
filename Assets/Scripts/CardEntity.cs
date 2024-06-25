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

    public CardType cardType; // �J�[�h�^�C�v�̑I��
    [Multiline] public string description; // ���ʃe�L�X�g�̓��͉ӏ�

    [Space(15)] // �X�y�[�X���󂯂�
    [Header("�� ���ʐݒ�")]
    public bool isSpeedAttacker; // �X�s�[�h�A�^�b�J�[(�����A�����������Ȃ�)���ǂ���
    public int drawCardNum; // �J�[�h����������
    public int[] addCardsList; // ��D�ɉ�����J�[�h�̃��X�g
    public int[] summonCardsList; // ��������J�[�h�̃��X�g
    public int chgMyLeaderHpNum; // �������[�_�[��HP��ω�������l
    public int chgEnemyLeaderHpNum; // ���胊�[�_�[��HP��ω�������l
    public int manaBoostNum; // �}�i�|�C���g(PP)�̏㏸�l
}

public enum CardType // �J�[�h�^�C�v�̎�ނ̒�`
{
    Monster,
    Spell
}