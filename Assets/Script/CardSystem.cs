using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    #region Variable
    [SerializeField] GameObject SelectedCard = null;
    [SerializeField] GameObject UnSelectedCard = null;
    [SerializeField] List<GameObject> Cards;
    [SerializeField] List<Vector3> Card_Position;
    [SerializeField] List<int> Card_Number;
    private Vector2 m_pos;
    private RaycastHit2D hit;
    private bool isSelect;
    private float time;
    private int positionIndex;
    private int cardIndex;
    #endregion

    #region UnityMethod
    private void Start()
    {
        ViewUpdate();
    }
    private void Update()
    {
        SelectCard();
        MoveCard();
    }
    #endregion

    #region Cards
    private void SelectCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(m_pos, Vector2.zero);
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
                            StartCoroutine(ClickCard(Cards[i].transform, 1f));
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

    private void MoveCard()
    {
        if (Input.GetMouseButton(0))
        {
            m_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            time += Time.deltaTime;
            if (time >= 0.2f)
            {
                isSelect = true;
                if (hit.collider != null)
                {
                    var cardPos = SelectedCard.transform.position;
                    SelectedCard.transform.position = new Vector3(m_pos.x, cardPos.y, cardPos.z);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            time = 0;
            if (isSelect)
            {
                isSelect = false;
                Down(SelectedCard.transform);
                var CardPos = SelectedCard.transform.localPosition;
                float shotDis = Mathf.Abs(Vector2.Distance(CardPos, Card_Position[0]));
                for (int i = 0; i < Card_Position.Count; i++)
                {
                    float distance = Mathf.Abs(Vector2.Distance(CardPos, Card_Position[i]));
                    if (distance <= shotDis)
                    {
                        positionIndex = i;
                        shotDis = distance;
                    }
                    if (SelectedCard == Cards[i])
                        cardIndex = i;
                }
                SelectedCard.transform.localPosition = Card_Position[positionIndex];
                Cards.Insert(positionIndex, Cards[cardIndex]);
                Cards.RemoveAt(cardIndex+1);
                ViewUpdate();
            }
        }
    }

    private void ViewUpdate()
    {
        var cardsize = Cards[0].transform.localScale.x * 0.3f;
        var size = cardsize * (Cards.Count + 1) / 2;
        for (int i = 0; i < Cards.Count; i++)
        {
            var sp = Cards[i].GetComponent<SpriteRenderer>();
            sp.sortingOrder = i;
            Card_Position[i] = new Vector3(-size + ((i + 1) * cardsize), Cards[i].transform.localPosition.y, -i);
            Cards[i].transform.localPosition = Card_Position[i];
        }
    }

    #endregion

    #region Other Func
    private IEnumerator Up(Transform obj, float distance)
    {
        while (obj.localPosition.y < distance)
        {
            yield return new WaitForSeconds(0.01f);
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, obj.localPosition + new Vector3(0, distance, 0), 0.15f);
        }
        obj.localPosition = new Vector3(obj.localPosition.x, distance, obj.localPosition.z);
    }
    private void Down(Transform obj)
    {
        obj.localPosition = new Vector3(obj.localPosition.x, 0, obj.localPosition.z);
    }

    private IEnumerator ClickCard(Transform obj, float distance)
    {
        StartCoroutine(Up(obj, distance));
        if (!isSelect)
        {
            yield return new WaitForSeconds(5f);
            Down(obj);
        }
    }
    #endregion
}