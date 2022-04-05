using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<Card> cardsPlayer = new List<Card>();
    public List<Card> cardsIA = new List<Card>();
    public GameObject parentPlayerDeck;
    public GameObject parentIADeck;
    public GameObject cardPrefab;
    public GameObject infoClone;

    public PlacedCards placedCards;

    void Awake()
    {
        CardsCreation();
    }

    public void CardsCreation()
    {
        for (int i = 0; i < cardsPlayer.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab);
            Card cardProperties = Instantiate(cardsPlayer[i]);
            go.GetComponent<CardDisplay>().card = cardProperties;
            go.name = "Carte joueur " + go.GetComponent<CardDisplay>().card.frenchName;
            go.transform.SetParent(parentPlayerDeck.transform, false);
            go.GetComponent<OnClickCard>().placedCards = placedCards;
            go.GetComponent<CardDisplay>().card.isEnnemy = false;
            go.GetComponent<OnClickCard>().infoClone = infoClone;
            infoClone.SetActive(false);
        }

        for (int i = 0; i < cardsIA.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab);
            go.GetComponent<CardDisplay>().card = GameObject.Instantiate(cardsIA[i]);
            go.name = "Carte IA " + go.GetComponent<CardDisplay>().card.frenchName;
            go.transform.SetParent(parentIADeck.transform, false);
            go.GetComponent<OnClickCard>().placedCards = placedCards;
            go.GetComponent<BoxCollider2D>().enabled = false;
            go.GetComponent<CardDisplay>().card.isEnnemy = true;
            go.GetComponent<CardDisplay>().visual.SetActive(false);
            go.GetComponent<CardDisplay>().cardIA.SetActive(true);
        }
    }

    //private Card GetRandomCard()
    //{
    //    int rnd = Random.Range(0, cards.Count);
    //    return cards[rnd];
    //}
}
