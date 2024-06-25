using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform cardParent; // ���݂̃J�[�h�̐e�I�u�W�F�N�g
    bool canDrag = true; // �J�[�h�𓮂����邩�ǂ����̃t���O

    Transform oriCardParent; // �h���b�O�J�n���̐e�I�u�W�F�N�g
    int cardIndex; // �h���b�O�J�n���̃J�[�h�̏���

    // �h���b�O���n�߂�Ƃ��ɍs������
    public void OnBeginDrag(PointerEventData eventData)
    {
        CardController card = GetComponent<CardController>(); // �J�[�h�R���g���[���[���擾
        canDrag = true;

        // �v���C���[�̃J�[�h�łȂ���΃h���b�O�s��
        if (!card.model.PlayerCard)
        {
            canDrag = false;
        }

        // ��D�̃J�[�h�̏ꍇ
        if (card.model.FieldCard == false)
        {
            // �g�p�\�łȂ��ꍇ�h���b�O�s��
            if (card.model.canUse == false)
            {
                canDrag = false;
            }
        }
        else // �t�B�[���h�̃J�[�h�̏ꍇ
        {
            // �U���\�łȂ��ꍇ�h���b�O�s��
            if (card.model.canAttack == false)
            {
                canDrag = false;
            }
        }

        // �h���b�O�s�Ȃ珈�����I��
        if (!canDrag)
        {
            return;
        }

        // �e�I�u�W�F�N�g�ƃC���f�b�N�X��ۑ�
        cardParent = transform.parent;
        oriCardParent = transform.parent;
        cardIndex = this.transform.GetSiblingIndex();

        // �e�I�u�W�F�N�g��ύX���ă��C�L���X�g�𖳌���
        transform.SetParent(cardParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // �h���b�O���̏���
    public void OnDrag(PointerEventData eventData)
    {
        // �h���b�O�s�Ȃ珈�����I��
        if (!canDrag)
        {
            return;
        }

        // �J�[�h�̈ʒu���X�V
        transform.position = eventData.position;
    }

    // �h���b�O���I�������Ƃ��̏���
    public void OnEndDrag(PointerEventData eventData)
    {
        // �h���b�O�s�Ȃ珈�����I��
        if (!canDrag)
        {
            return;
        }

        // �e�I�u�W�F�N�g�����ɖ߂�
        transform.SetParent(cardParent, false);

        // ���̐e�Ɠ����Ȃ猳�̃C���f�b�N�X�ɖ߂�
        if (oriCardParent == cardParent)
        {
            transform.SetSiblingIndex(cardIndex);
        }

        // ���C�L���X�g��L����
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    // �U�����[�V�����̃R���[�`��
    public IEnumerator AttackMotion(Transform target)
    {
        Vector3 currentPosition = transform.position; // ���݂̈ʒu��ۑ�
        cardParent = transform.parent;
        int attackCardIndex = transform.GetSiblingIndex(); // �q�v�f�̏��ԏ����擾

        // �ꎞ�I�ɐe�I�u�W�F�N�g��ύX
        transform.SetParent(cardParent.parent);

        // �^�[�Q�b�g�Ɍ������Ĉړ�
        transform.DOMove(target.position, 0.75f).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(0.75f);

        // �U�����̉���炷
        SoundManager.instance.PlaySE(1);

        // ���̈ʒu�ɖ߂�
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);

        // �e�I�u�W�F�N�g�����ɖ߂��A�C���f�b�N�X��ݒ�
        transform.SetParent(cardParent);
        transform.SetSiblingIndex(attackCardIndex);
    }

    // �������[�V�����̃R���[�`��
    public IEnumerator SummonMotion()
    {
        // �������̉����o��
        SoundManager.instance.PlaySE(2);

        // 0.5�b������1.2�{�̑傫���ɂ���
        transform.DOScale(1.2f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 0.5�b�����Č��̑傫���ɂ���
        transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);
    }

    // �X�y���g�p���[�V�����̃R���[�`��
    public IEnumerator UseSpellMotion()
    {
        // �X�y���g�p���̉����o��
        SoundManager.instance.PlaySE(3);

        yield return new WaitForSeconds(0.1f);

        // �ꎞ�I�ɐe�I�u�W�F�N�g��ύX
        transform.SetParent(transform.root);

        // 0.5�b������3�{�̑傫���ɂ���
        transform.DOScale(3.0f, 0.5f);
        yield return new WaitForSeconds(0.7f);

        // 0.3�b������5�{�̑傫���ɂ���
        transform.DOScale(5.0f, 0.3f);
        SoundManager.instance.PlaySE(4);
        yield return new WaitForSeconds(0.3f);

        // 0�b�ŃT�C�Y��0�ɂ���
        transform.DOScale(0.0f, 0.0f);
        yield return new WaitForSeconds(0.0f);
    }
}
