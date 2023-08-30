using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsGrounded = true;
    private float moveSpeed = 10f;
    private float jumpSpeed = 300f;

    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        transform.localPosition = Vector3.zero;
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpSpeed);
    }

    public void Move(Vector3 moveDir)
    {
        rb.velocity = moveDir * moveSpeed;
    }

    public void SetUI(string name, bool isReady, bool Leader)
    {
        TMP_Text[] texts = gameObject.GetComponentsInChildren<TMP_Text>();
        texts[0].text = name;
        texts[1].text = (Leader || isReady) ? "READY!" : "WAIT..";
        texts[2].text = (Leader) ? "LEAD" : "";
    }
}
