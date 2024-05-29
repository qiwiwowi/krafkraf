using UnityEngine;

public class Background : MonoBehaviour
{
    public background backgroundType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            InteractionKeyDown.instance.SetPosition(true, transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionKeyDown.instance.SetPosition(false, Vector3.one);
        }
    }
}
