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

    public void Show(CardModel cardModel) // cardModelのデータ取得と反映
    {
        nameText.text = cardModel.name;
        powerText.text = cardModel.power.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        descriptionText.text = cardModel.description;

        if (cardModel.cardType == CardType.Spell)
        {
            cardPanel.GetComponent<Image>().color = new Color32(200, 150, 255, 255); // ４つめの値は透明度、0で透明になる
            //powerText.enabled = false;　// 下記の処理と重複するのでコメントアウト
            powerPanel.SetActive(false);
        }
    }

    public void SetCanAttackPanel(bool flag)
    {
        canAttackPanel.SetActive(flag);
    }

    public void SetCanUsePanel(bool flag) // フラグに合わせてCanUsePanelを付けるor消す
    {
        canUsePanel.SetActive(flag);
    }
}