using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public static WebCamTexture WebcamTexture;
    public static Texture2D Texture;

    public GameObject IRCamera;

    // Use this for initialization
    void Start () {
        WebcamTexture = new WebCamTexture(1080, 1440);
        Material mat = Instantiate(IRCamera.GetComponent<RawImage>().material);
        mat.mainTexture = WebcamTexture;
        IRCamera.GetComponent<RawImage>().material = mat;
        WebcamTexture.Play();
    }
}
