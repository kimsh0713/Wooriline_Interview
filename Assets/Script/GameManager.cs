using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject SelectedCard = null;
    public GameObject UnSelectedCard = null;
    public List<GameObject> Cards;
    private int index;

    void Update()
    {
        ViewUpdate();
        SelectCard();
    }

    private void SelectCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null)
            {
                SelectedCard = hit.collider.gameObject;

                if (SelectedCard != UnSelectedCard)
                {
                    UnSelectedCard = SelectedCard;
                    for (int i = 0; i < Cards.Count; i++)
                    {
                        if (Cards[i] == SelectedCard)
                        {
                            StopCoroutine(ClickCard(Cards[i].transform, 1f));
                            StartCoroutine(ClickCard(Cards[i].transform, 1f));
                            //StartCoroutine(Up(Cards[i].transform, 1f));
                        }
                        else
                        {
                            Down(Cards[i].transform);
                        }
                    }
                }
                else
                {
                    UnSelectedCard = null;
                    Down(SelectedCard.transform);
                }
            }
        }
    }

    private void ViewUpdate()
    {
        var cardsize = Cards[0].transform.localScale.x / 4;
        var size = cardsize * (Cards.Count + 1) / 2;
        for (int i = 0; i < Cards.Count; i++)
        {
            var sp = Cards[i].GetComponent<SpriteRenderer>();
            sp.sortingOrder = i;
            Cards[i].transform.localPosition = new Vector3(-size + ((i + 1) * cardsize), Cards[i].transform.localPosition.y, -i);
        }
    }

    public IEnumerator Up(Transform obj, float distance)
    {
        while (obj.localPosition.y < distance)
        {
            yield return new WaitForSeconds(0.01f);
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, obj.localPosition + new Vector3(0, distance, 0), 0.15f);
        }
        obj.localPosition = new Vector3(obj.localPosition.x, distance, obj.localPosition.z);
    }
    public void Down(Transform obj)
    {
        obj.localPosition = new Vector3(obj.localPosition.x, 0, obj.localPosition.z);
    }

    private IEnumerator ClickCard(Transform obj, float distance)
    {
        StartCoroutine(Up(obj, distance));
        yield return new WaitForSeconds(5f);
        Down(obj);
    }
}