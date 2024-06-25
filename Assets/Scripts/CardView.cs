using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] Text nameText, powerText, costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject canAttackPanel, canUsePanel;
    [SerializeField] Text descriptionText;
    [SerializeField] GameObject cardPanel;
    [SerializeField] GameObject powerPanel;

    public void Show(CardModel cardModel) // cardModel�̃f�[�^�擾�Ɣ��f
    {
        nameText.text = cardModel.name;
        powerText.text = cardModel.power.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        descriptionText.text = cardModel.description;

        if (cardModel.cardType == CardType.Spell)
        {
            cardPanel.GetComponent<Image>().color = new Color32(200, 150, 255, 255); // �S�߂̒l�͓����x�A0�œ����ɂȂ�
            //powerText.enabled = false;�@// ���L�̏����Əd������̂ŃR�����g�A�E�g
            powerPanel.SetActive(false);
        }
    }

    public void SetCanAttackPanel(bool flag)
    {
        canAttackPanel.SetActive(flag);
    }

    public void SetCanUsePanel(bool flag) // �t���O�ɍ��킹��CanUsePanel��t����or����
    {
        canUsePanel.SetActive(flag);
    }
}