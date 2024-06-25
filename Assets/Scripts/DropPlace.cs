using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �t�B�[���h�ɃA�^�b�`����N���X
public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) // �h���b�v���ꂽ���ɍs������
    {
        //CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>(); // �h���b�O���Ă�����񂩂�CardMovement���擾
        CardController card = eventData.pointerDrag.GetComponent<CardController>(); // ����̏�����������

        if (card != null) // �����J�[�h������΁A
        {
            if (card.model.canUse == true)  // �g�p�\�ȃJ�[�h�Ȃ�A
            {
                card.movement.cardParent = this.transform; // �J�[�h�̐e�v�f�������i�A�^�b�`����Ă�I�u�W�F�N�g�j�ɂ��� ����̏�����������
                card.DropField(); // �J�[�h���t�B�[���h�ɒu�������̏������s��
            }
        }
    }
}