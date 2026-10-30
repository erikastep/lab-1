using UnityEngine;

// Switch cameras with keys 1-4
public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;
    string[] names = { "Code Agent", "Graph Agent", "Flocking", "All" };
    int current = 3;

    void Start()
    {
        Show(current);
    }

    void Update()
    {
        // Keys 1-4
        for (int i = 0; i < cameras.Length; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                Show(i);
    }

    // Turn on one camera, turn off the rest
    void Show(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].gameObject.SetActive(i == index);
        current = index;
    }

    // Camera name on screen
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 25), "Camera " + (current + 1) + ": " + names[current] + "  (press 1-4)");
    }
}
