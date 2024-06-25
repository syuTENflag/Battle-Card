using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject changeTurnPanel;
    [SerializeField] Text changeTurnText;
    [SerializeField] GameObject restartGameButton;

    public IEnumerator ShowChangeTurnPanel()
    {
        changeTurnPanel.SetActive(true);
        changeTurnText.color = Color.white;

        if (GameManager.instance.isPlayerTurn == true)
        {
            changeTurnText.text = "Your Turn";
        }
        else
        {
            changeTurnText.text = "Enemy Turn";
        }

        yield return new WaitForSeconds(2);

        changeTurnPanel.SetActive(false);
    }

    public void ShowGameEndPanel(bool isPlayerWinnig)
    {
        changeTurnPanel.SetActive(true);
        restartGameButton.SetActive(true);

        if (isPlayerWinnig == true)
        {
            changeTurnText.color = Color.yellow;
            changeTurnText.text = "You Win!";
        }
        else
        {
            changeTurnText.color = Color.red;
            changeTurnText.text = "You Lose...";
        }
    }

    public void HideGameEndPanel()
    {
        restartGameButton.SetActive(false);
        changeTurnPanel.SetActive(false);
    }

}