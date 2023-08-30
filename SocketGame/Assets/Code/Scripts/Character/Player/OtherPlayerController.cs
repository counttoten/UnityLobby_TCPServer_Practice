using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void Jump()
    {

        player.Jump();
    }

    private void Move(Vector3 _move)
    {
        player.Move(_move);
    }
}
