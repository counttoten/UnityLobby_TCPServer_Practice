using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void Update()
    {
        Jump();
        Move();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded)
        {
            // jump animator
            player.Jump();
        }
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) ||  Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            player.Move(dir);
        }
    }
}
