using UnityEngine;

public class GameColliders : MonoBehaviour
{
    [SerializeField]
    Transform parentObject;

    [SerializeField]
    GameObject userPixelPrefab;
    [SerializeField]
    GameObject bottomLinePrefab;

    GameObject[,] colliderObjects;

    int cols = 0;
    int rows = 0;


    [Range (0.1f, 1)]
    [SerializeField]
    float colliderDetails = 1f;


    public void CreateColliders(int colsImage, int imageRows)
    {
        cols = (int)(colliderDetails * colsImage);
        rows = (int)(colliderDetails * imageRows);

        colliderObjects = new GameObject[cols, rows];

        float imageScale = Mathf.Min((float)Screen.width / cols, (float)Screen.height / rows);

        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                GameObject currentCollider = Instantiate(userPixelPrefab);

                currentCollider.transform.SetParent(parentObject, false);
                currentCollider.transform.localPosition = new Vector3((cols / 2 - c) * imageScale, (rows / 2 - r) * imageScale, 0);
                currentCollider.transform.localScale = Vector3.one * imageScale;

                colliderObjects[c, r] = currentCollider;
            }
        }

        GameObject bottomLine = Instantiate(bottomLinePrefab);
        bottomLine.transform.SetParent(parentObject, false);
        bottomLine.transform.localPosition = new Vector3(0, -(rows / 2) * imageScale, 0);
        bottomLine.transform.localScale = new Vector3(imageScale * cols, imageScale, imageScale);
    }

    public void UpdateFrame(nuitrack.UserFrame frame)
    {
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                ushort userId = frame[(int)(r / colliderDetails), (int)(c / colliderDetails)];

                if (userId == 0)
                    colliderObjects[c, r].SetActive(false);
                else
                    colliderObjects[c, r].SetActive(true);
            }
        }
    }
}