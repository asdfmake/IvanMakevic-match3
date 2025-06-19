using UnityEngine;

public class Kocka : MonoBehaviour
{
    public Boja.boje boja;
    public int i;
    public int j;

    // MoveTo variables
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

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
}
