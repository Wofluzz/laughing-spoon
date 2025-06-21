using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Transform cam; // La caméra à suivre
    private Vector3 camStartPos;

    GameObject[] backgrounds;
    Material[] mats;
    Vector3[] startPos;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.5f)]
    public float parallaxSpeed = 0.1f;

    void Start()
    {
        camStartPos = cam.position;

        int count = transform.childCount;
        backgrounds = new GameObject[count];
        mats = new Material[count];
        startPos = new Vector3[count];
        backSpeed = new float[count];

        // Récupération des enfants
        for (int i = 0; i < count; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mats[i] = backgrounds[i].GetComponent<Renderer>().material;
            startPos[i] = backgrounds[i].transform.position;
        }

        // Calcul des vitesses relatives
        CalculateBackSpeeds(count);
    }

    void CalculateBackSpeeds(int count)
    {
        farthestBack = 0f;
        for (int i = 0; i < count; i++)
        {
            float depth = Mathf.Abs(backgrounds[i].transform.position.z - cam.position.z);
            if (depth > farthestBack) farthestBack = depth;
        }

        for (int i = 0; i < count; i++)
        {
            float depth = Mathf.Abs(backgrounds[i].transform.position.z - cam.position.z);
            backSpeed[i] = 1f - (depth / farthestBack);
        }
    }

    void LateUpdate()
    {
        Vector3 camDelta = cam.position - camStartPos;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;

            Vector3 newWorldPos = startPos[i] + camDelta * speed;
            backgrounds[i].transform.position = new Vector3(newWorldPos.x, newWorldPos.y, startPos[i].z);

            mats[i].SetTextureOffset("_MainTex", new Vector2(camDelta.x * speed * 0.1f, 0));
        }
    }
}
