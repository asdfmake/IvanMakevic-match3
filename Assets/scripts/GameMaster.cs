using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField]
    public GameObject testCube;

    void Start()
    {
        Vector3[,] grid = new Vector3[6, 6];
        GameObject[,] kocke = new GameObject[6, 6];
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
                kocke[row, col] = obj;
                // todo dodaj logiku na svaku kocku koja ce da zna na kom se indeksu nalazi ili nesto slicno
                // todo neka se kocka instancira iznad ekrana, a posle, preko funkcije dobija poziciju na koju treba da ide
                // todo dodaj svaku kocku na jos jednu duplu listu game objekata
            }
            
        }
    }

    void CheckMatch()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //todo dodaj logiku za selektovani objekat, i objekat koji se nakon njega klikne, da li je match blabla
    }
}
