using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [SerializeField]
    int scoreValue = 5;

    bool active = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!active)
            return;

        active = false;

        Destroy(gameObject);

        if (collision.transform.tag == "UserPixel")
            GameProgress.instance.AddScore(scoreValue);
        else if (collision.transform.tag == "BottomLine")
            GameProgress.instance.RemoveScore(scoreValue);
    }
}
