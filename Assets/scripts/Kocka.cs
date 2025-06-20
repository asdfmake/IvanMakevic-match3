using UnityEngine;

public class Kocka : MonoBehaviour
{
    public Boja.boje boja;
    public int i;
    public int j;

    // MoveTo variables
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    public bool isMoving = false;

    void Start()
    {
        boja = Boja.GetRandomBoja();
        GetComponent<Renderer>().material.color = Boja.GetBojaFromEnum(boja);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector3 moveToPos)
    {
        targetPosition = moveToPos;
        isMoving = true;
    }

    public bool NextTo(GameObject other)
    {
        Kocka k = other.GetComponent<Kocka>();
        int dx = Mathf.Abs(this.i - k.i);
        int dy = Mathf.Abs(this.j - k.j);

        // Ako su isti objekti
        if (dx == 0 && dy == 0)
            return false;

        // Ako su direktno susedni (levo/desno ili gore/dole)
        if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
            return true;

        return false;
    }

    public void SetCoordinates(int i, int j)
    {
        this.i = i;
        this.j = j;
    }
}
