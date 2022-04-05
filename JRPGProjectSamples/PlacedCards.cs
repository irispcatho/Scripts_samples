using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class PlacedCards : MonoBehaviour
{
    public static PlacedCards instance;
    public List<GameObject> placedCardsList;
    public List<GameObject> OrderList;
    public CasesManager casesManager;
    public PlayerDeck playerDeck;
    public TutoManager tuto;

    public GameObject[] walls;
    private int pdvPlayer;
    private int pdvIA;
    private int numberCardsPlayer;
    private int numberCardsIA;
    public int round = 0;
    public GameObject lastCardClicked;
    public GameObject infoClone;
    public int whoWon = -1;
    public int numberWinPlayer = 0;
    public int numberWinIA = 0;

    public Upgrade[] upgrade;

    public List<PatternAttack> patternAttacks;

    bool launchedattack = false;

    private void Start()
    {
        instance = this;
        AudioManager.instance.Play("Combat");
        Scene scene = SceneManager.GetSceneByName("FightTuto");
        if (SceneManager.GetActiveScene() != scene)
        {
            for (int i = 0; i < DialogManager.instance.walls.Length; i++)
            {
                walls[i] = DialogManager.instance.walls[i];
            }
        }
    }
    public void Update()
    {
        if (OrderList.Count >= 12 && launchedattack == false)
        {
            StartCoroutine(Attack());
            launchedattack = true;
        }
        Damage();
    }

    public PatternAttack GetPattern(string name)
    {
        foreach (var item in patternAttacks)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }

    public IEnumerator Attack()
    {
        for (int i = 0; i <= OrderList.Count - 1; i++)
        {

            if (i == OrderList.Count - 1 && OrderList[i].GetComponent<CardDisplay>().card.isDead)
            {
                FinishTour(i);
                break;
            }

            for (int j = 0; j <= OrderList.Count - 1; j++)
            {
                if (j > 0)
                {
                    if (OrderList[j] != OrderList[j - 1])
                    {
                        OrderList[j - 1].GetComponent<CardDisplay>().cadreP.SetActive(false);
                        OrderList[j - 1].GetComponent<CardDisplay>().cadreIA.SetActive(false);
                    }
                    if (j == OrderList.Count - 1 && OrderList[j].GetComponent<CardDisplay>().card.isDead)
                        break;
                }
            }

            if (OrderList[i].GetComponent<CardDisplay>().card.isEnnemy)
            {
                if (OrderList[i].GetComponent<CardDisplay>().card.powerIA <= 0)
                {
                    print("carte morte");
                    i++;
                }
            }
            else
            {
                if (OrderList[i].GetComponent<CardDisplay>().card.powerPlayer <= 0)
                {
                    print("carte morte");
                    i++;
                }
            }

            CaseSlot slot = OrderList[i].GetComponent<CardDisplay>().card.cell;
            GameObject cadreP = OrderList[i].GetComponent<CardDisplay>().cadreP;
            GameObject cadreIA = OrderList[i].GetComponent<CardDisplay>().cadreIA;
            cadreP.SetActive(true);
            cadreIA.SetActive(true);

            gameObject.GetComponent<PlacedCards>().infoClone = infoClone;
            InfosCard infosCard = infoClone.GetComponent<InfosDisplay>().infosCard;
            Card vars = OrderList[i].GetComponent<CardDisplay>().card;
            infosCard.cardName = vars.frenchName;
            if (!vars.isEnnemy)
                infosCard.power = vars.powerPlayer;
            if (vars.isEnnemy)
                infosCard.power = vars.powerIA;
            infosCard.order = vars.gameOrder;
            infosCard.description = vars.description;
            infosCard.pattern = vars.pattern;


            #region AttackPattern

            PatternAttack pattern = GetPattern(OrderList[i].GetComponent<CardDisplay>().card.frenchName);
            Vector2 cardToAttack = new Vector2(slot.coordinates.x, slot.coordinates.y);

            foreach (var item in pattern.position)
            {
                if (!slot.card.isEnnemy)
                    casesManager.DetectCard(pattern, slot, slot.card.powerPlayer, cardToAttack, item.x, item.y);
                else
                    casesManager.DetectCard(pattern, slot, slot.card.powerIA, cardToAttack, item.x, item.y);
            }

            FinishTour(i);
            yield return new WaitForSeconds(2);

            #endregion
        }
    }

    private void FinishTour(int i)
    {
        if (i >= 11)
        {
            for (int j = 0; j <= placedCardsList.Count - 1; j++)
            {
                if (placedCardsList[j].GetComponent<CardDisplay>().card.isEnnemy == false)
                {
                    numberCardsPlayer++;
                    pdvPlayer += placedCardsList[j].GetComponent<CardDisplay>().card.powerPlayer;
                }
                else
                {
                    numberCardsIA++;
                    pdvIA += pdvPlayer += placedCardsList[j].GetComponent<CardDisplay>().card.powerIA;
                }
            }
            if (numberCardsPlayer > numberCardsIA)
            {
                whoWon = 0;
                numberWinPlayer++;
                print("Le joueur a gagné");
            }
            else if (numberCardsPlayer < numberCardsIA)
            {
                whoWon = 1;
                numberWinIA++;
                print("L'IA a gagné");
            }
            else if (numberCardsPlayer == numberCardsIA)
            {
                if (pdvPlayer > pdvIA)
                {
                    whoWon = 0;
                    numberWinPlayer++;
                    print("Le joueur a gagné");
                }
                else if (pdvPlayer < pdvIA)
                {
                    whoWon = 1;
                    numberWinIA++;
                    print("L'IA a gagné");
                }
            }

            StartCoroutine(WaitToUpdateRound());

        }

    }
    private void Damage()
    {
        for (int c = 0; c <= OrderList.Count - 1; c++) //show damage
        {
            CardDisplay cardDisplay = OrderList[c].GetComponent<CardDisplay>();
            cardDisplay.onCaseTextIAPower.text = cardDisplay.card.powerIA.ToString();
            cardDisplay.onCaseTextPower.text = cardDisplay.card.powerPlayer.ToString();

            cardDisplay.damageGoPText.text = cardDisplay.card.damage.ToString();
            cardDisplay.damageGoIAText.text = cardDisplay.card.damage.ToString();

            cardDisplay.signeDamageGoPText.text = cardDisplay.card.signeDamage.ToString();
            cardDisplay.signeDamageGoIAText.text = cardDisplay.card.signeDamage.ToString();

            if (cardDisplay.card.showDamage)
            {
                cardDisplay.damageGoP.SetActive(true);
                cardDisplay.damageGoIA.SetActive(true);
            }
            else
            {
                cardDisplay.damageGoP.SetActive(false);
                cardDisplay.damageGoIA.SetActive(false);
            }
            if (cardDisplay.card.powerPlayer <= 0 || cardDisplay.card.powerIA <= 0)
            {
                cardDisplay.card.powerPlayer = 0;
                cardDisplay.card.powerIA = 0;
                cardDisplay.card.isDead = true;
                cardDisplay.onCaseIA.SetActive(false);
                cardDisplay.onCase.SetActive(false);
                if (cardDisplay.card.isEnnemy == false)
                    cardDisplay.imageDeadP.SetActive(true);
                else
                    cardDisplay.imageDeadIA.SetActive(true);
                OrderList[c].GetComponent<BoxCollider2D>().enabled = false;
                placedCardsList.Remove(OrderList[c]);
            }
        }
    }
    public void ResetCards()
    {
        placedCardsList.Clear();
        OrderList.Clear();
        casesManager.CasesListUsed.Clear();
        lastCardClicked = gameObject;
        for (int i = playerDeck.parentPlayerDeck.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(playerDeck.parentPlayerDeck.transform.GetChild(i).gameObject);
        }

        for (int i = playerDeck.parentIADeck.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(playerDeck.parentIADeck.transform.GetChild(i).gameObject);
        }
        playerDeck.CardsCreation();
        numberCardsPlayer = 0;
        numberCardsIA = 0;
        launchedattack = false;
    }

    private IEnumerator WaitToUpdateRound()
    {
        yield return new WaitForSeconds(1.5f);
        if (numberWinPlayer == 2 || numberWinIA == 2)
            StartCoroutine(WaitForClose());
        whoWon = -1;
        round++;

        if (round < 3)
            ResetCards();
        else if (round >= 3)
            StartCoroutine(WaitForClose());
    }

    IEnumerator WaitForClose()
    {
        yield return new WaitForSeconds(1.95f);
        Scene scene = SceneManager.GetSceneByName("FightTuto");
        if (SceneManager.GetActiveScene() == scene)
        {
            tuto.dialogUI.SetActive(true);
            tuto.DisplayNextSentenceTuto();
        }
        round = 0;
        whoWon = -1;
        if (SceneManager.GetActiveScene() != scene)
        {
            if (numberWinPlayer == 2)
            {
                DialogManager.instance.DisplayNextSentence();
                CloseScene(1, "Fight1", 0);
                CloseScene(2, "Fight2", 1);
                CloseScene(3, "Fight3", 2);
                DialogManager.instance.combatAlreadyLauched = false;
            }
            else if(numberWinIA == 2)
            {
                DialogManager.instance.EndDialog();
                CloseScene(1, "Fight1", 0);
                CloseScene(2, "Fight2", 1);
                CloseScene(3, "Fight3", 2);
                DialogManager.instance.combatAlreadyLauched = false;
            }
        }
    }

    private void CloseScene(int whichCombat, string whichScene, int whichUpgrade)
    {
        AudioManager.instance.Play("Exploration");
        AudioManager.instance.Stop("Combat");
        if (DialogManager.instance.currentCombat == whichCombat)
        { 
            if (numberWinPlayer == 2)
            {
                if (DialogManager.instance.currentCombat < 3)
                    walls[whichCombat - 1].SetActive(false);
                if (!upgrade[whichUpgrade].asBeenDiscovered)
                {
                    upgrade[whichUpgrade].cardAffected.powerPlayer += upgrade[whichUpgrade].attackUpgrade;
                    upgrade[whichUpgrade].asBeenDiscovered = true;
                }
            }
            SceneManager.UnloadSceneAsync(whichScene);
        }
    }
    public IEnumerator Damage(Card card)
    {
        yield return new WaitForSeconds(2);
        card.showDamage = false;
    }
}