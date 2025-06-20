using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public GameObject testCube;
    public Vector3[,] grid = new Vector3[6, 6];
    public GameObject[,] kocke = new GameObject[6, 6];
    public int score = 0;

    public GameObject selected;

    void Start()
    {
        float x = -2.5f;
        float y = -2.5f;

        // Podesavanje grid-a pocetnim pozicijama
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                grid[i, j] = new Vector3(x, y, 0);
                x += 1;
            }
            x = -2.5f;
            y += 1;
        }

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                GameObject obj = Instantiate(testCube, grid[row, col], Quaternion.identity);
                obj.GetComponent<Kocka>().SetCoordinates(row, col);
                kocke[row, col] = obj;
                // todo neka se kocka instancira iznad ekrana, a posle, preko funkcije dobija poziciju na koju treba da ide
                // todo instanciranje pamti poslednjih dve poje kockica i proverava da li na pocetku ima match, ako ima, menja boju kocke
            }
            
        }

        CheckMatch();
    }

    bool CheckMatch()
    {
        List<Kocka> toDestroy = new List<Kocka>();
        bool[,] alreadyMarked = new bool[6, 6];

        // Horizontalni match
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Boja.boje currentBoja = kocke[i, j].GetComponent<Kocka>().boja;
                int count = 1;

                // Broji koliko ih ima u nizu
                for (int k = j + 1; k < 6; k++)
                {
                    if (kocke[i, k].GetComponent<Kocka>().boja == currentBoja)
                        count++;
                    else
                        break;
                }

                if (count >= 3)
                {
                    for (int x = 0; x < count; x++)
                    {
                        if (!alreadyMarked[i, j + x])
                        {
                            toDestroy.Add(kocke[i, j + x].GetComponent<Kocka>());
                            alreadyMarked[i, j + x] = true;
                        }
                    }
                }

                j += count - 1;
            }
        }

        // Vertikalni match
        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                Boja.boje currentBoja = kocke[i, j].GetComponent<Kocka>().boja;
                int count = 1;

                for (int k = i + 1; k < 6; k++)
                {
                    if (kocke[k, j].GetComponent<Kocka>().boja == currentBoja)
                        count++;
                    else
                        break;
                }

                if (count >= 3)
                {
                    for (int x = 0; x < count; x++)
                    {
                        if (!alreadyMarked[i + x, j])
                        {
                            toDestroy.Add(kocke[i + x, j].GetComponent<Kocka>());
                            alreadyMarked[i + x, j] = true;
                        }
                    }
                }

                i += count - 1;
            }
        }

        if (toDestroy.Count > 0)
        {
            StartCoroutine(DestroyAndDrop(toDestroy));
            score++;
            GameObject.FindGameObjectWithTag("Finish").GetComponent<TextMeshProUGUI>().text = "Score: " + score;

        }

        return toDestroy.Count > 0;
    }

    IEnumerator DestroyAndDrop(List<Kocka> toDestroy)
    {
        yield return new WaitForSeconds(.2f);

        foreach (Kocka k in toDestroy)
        {
            kocke[k.i, k.j] = null;
            Destroy(k.gameObject);
            //todo dodaj animaciju za unistavanje kockica
        }

        yield return new WaitForSeconds(0.2f);

        // Pomeri kocke dole
        for (int col = 0; col < 6; col++)
        {
            for (int row = 0; row < 6; row++)
            {
                if (kocke[row, col] == null)
                {
                    for (int above = row + 1; above < 6; above++)
                    {
                        if (kocke[above, col] != null)
                        {
                            Kocka pada = kocke[above, col].GetComponent<Kocka>();
                            kocke[row, col] = kocke[above, col];
                            kocke[above, col] = null;

                            pada.SetCoordinates(row, col);
                            pada.MoveTo(grid[row, col]);
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        // Dodaj nove kocke
        for (int col = 0; col < 6; col++)
        {
            for (int row = 0; row < 6; row++)
            {
                if (kocke[row, col] == null)
                {
                    Vector3 spawnPos = grid[row, col] + Vector3.up * 5f;
                    GameObject obj = Instantiate(testCube, spawnPos, Quaternion.identity);
                    Kocka novaKocka = obj.GetComponent<Kocka>();
                    novaKocka.SetCoordinates(row, col);
                    novaKocka.MoveTo(grid[row, col]);
                    kocke[row, col] = obj;
                }
            }
        }

        yield return new WaitForSeconds(0.5f); 

        // proveri nove za match
        CheckMatch();
    }




    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject noviSelected = hit.collider.gameObject;
                if (selected == null)
                {
                    selected = noviSelected;
                }
                // else se odnosi na logiku da li je moguace napraviti potez
                else
                {
                    // check da li se selected i novi selected nalaze jedno pored drugog
                    if (selected.GetComponent<Kocka>().NextTo(noviSelected))
                    {
                        StartCoroutine(ZameniIProveri(selected, noviSelected));
                    }

                    selected = null;
                    noviSelected = null;

                }

            }
        }
    }

    IEnumerator ZameniIProveri(GameObject a, GameObject b)
    {
        Kocka ka = a.GetComponent<Kocka>();
        Kocka kb = b.GetComponent<Kocka>();

        // originalne koordinate i pozicije
        int ai = ka.i, aj = ka.j;
        int bi = kb.i, bj = kb.j;

        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        // menjanje vrednosti
        ka.SetCoordinates(bi, bj);
        kb.SetCoordinates(ai, aj);

        kocke[ai, aj] = b;
        kocke[bi, bj] = a;

        ka.MoveTo(posB);
        kb.MoveTo(posA);

        // cekaj do se ne zavrsi pomeranje
        yield return new WaitUntil(() => !kaIsMoving() && !kbIsMoving());


        if (!CheckMatch())
        {
            // vracanje vrednosti na staro
            ka.SetCoordinates(ai, aj);
            kb.SetCoordinates(bi, bj);

            kocke[ai, aj] = a;
            kocke[bi, bj] = b;

            ka.MoveTo(posA);
            kb.MoveTo(posB);

            yield return new WaitUntil(() => !kaIsMoving() && !kbIsMoving());
        }

        bool kaIsMoving() => ka.isMoving;
        bool kbIsMoving() => kb.isMoving;
    }




}
