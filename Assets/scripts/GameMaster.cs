using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public GameObject testCube;
    public Vector3[,] grid = new Vector3[6, 6];
    public GameObject[,] kocke = new GameObject[6, 6];

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

    void CheckMatch()
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
                    Debug.Log($"Match horizontal at ({i}, {j}) length {count}");
                    for (int x = 0; x < count; x++)
                    {
                        if (!alreadyMarked[i, j + x])
                        {
                            toDestroy.Add(kocke[i, j + x].GetComponent<Kocka>());
                            alreadyMarked[i, j + x] = true;
                        }
                    }
                }

                j += count - 1; // presko?i ve? proverene
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
                    Debug.Log($"Match vertical at ({i}, {j}) length {count}");
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
        }
    }

    IEnumerator DestroyAndDrop(List<Kocka> toDestroy)
    {
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
                    Vector3 spawnPos = grid[row, col] + Vector3.up * 2f; // todo promeni vrednosti da izleda dobro
                    GameObject obj = Instantiate(testCube, spawnPos, Quaternion.identity);
                    Kocka novaKocka = obj.GetComponent<Kocka>();
                    novaKocka.SetCoordinates(row, col);
                    novaKocka.MoveTo(grid[row, col]);
                    kocke[row, col] = obj;
                }
            }
        }

        yield return new WaitForSeconds(0.2f); // todo promeni vrednosti da izleda dobro 

        // Automatski proveri nove za match
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
                // else se odnosi na logiku poredjenja
                else
                {
                    // check da li se selected i novi selected nalaze jedno pored drugog
                    if (selected.GetComponent<Kocka>().NextTo(noviSelected))
                    {

                        // check da li je boja ista
                        if (selected.GetComponent<Kocka>().boja == noviSelected.GetComponent<Kocka>().boja)
                        {
                            Debug.Log("ista boja");
                        }
                        else
                        {
                            Debug.Log("nije ista boja");
                        }

                    }

                    selected = null;
                    noviSelected = null;

                }

            }
        }
    }
}
