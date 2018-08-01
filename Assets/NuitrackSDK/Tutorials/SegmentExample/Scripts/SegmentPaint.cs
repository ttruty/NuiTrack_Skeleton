using UnityEngine;
using UnityEngine.UI;

public class SegmentPaint : MonoBehaviour
{
    [SerializeField]
    Color32[] colorsList;

    Rect imageRect;

    [SerializeField]
    Image segmentOut;

    Texture2D segmentTexture;
    Sprite segmentSprite;
    byte[] outSegment;


    int cols = 0;
    int rows = 0;

    [SerializeField]
    GameColliders gameColliders;
    [SerializeField]
    ObjectSpawner objectSpawner;

    void Start()
    {
        NuitrackManager.onUserTrackerUpdate += ColorizeUser;

        NuitrackManager.DepthSensor.SetMirror(true);

        nuitrack.OutputMode mode = NuitrackManager.DepthSensor.GetOutputMode();
        cols = mode.XRes;
        rows = mode.YRes;

        imageRect = new Rect(0, 0, cols, rows);

        segmentTexture = new Texture2D(cols, rows, TextureFormat.ARGB32, false);

        outSegment = new byte[cols * rows * 4];

        segmentOut.type = Image.Type.Simple;
        segmentOut.preserveAspect = true;

        gameColliders.CreateColliders(cols, rows);
        objectSpawner.StartSpawn(cols);
    }

    void OnDestroy()
    {
        NuitrackManager.onUserTrackerUpdate -= ColorizeUser;
    }

    string msg = "";

    void ColorizeUser(nuitrack.UserFrame frame)
    {
        if (frame.Users.Length > 0)
            msg = "User found";
        else
            msg = "User not found";

        for (int i = 0; i < (cols * rows); i++)
        {
            Color32 currentColor = colorsList[frame[i]];

            int ptr = i * 4;
            outSegment[ptr] = currentColor.a;
            outSegment[ptr + 1] = currentColor.r;
            outSegment[ptr + 2] = currentColor.g;
            outSegment[ptr + 3] = currentColor.b;
        }

        segmentTexture.LoadRawTextureData(outSegment);
        segmentTexture.Apply();

        segmentSprite = Sprite.Create(segmentTexture, imageRect, Vector3.one * 0.5f, 100f, 0, SpriteMeshType.FullRect);

        segmentOut.sprite = segmentSprite;

        gameColliders.UpdateFrame(frame);
    }  

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 50;
        GUILayout.Label(msg);
    }
}