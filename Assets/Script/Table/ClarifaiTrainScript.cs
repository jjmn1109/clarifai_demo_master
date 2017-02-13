using Clarifai;
using UnityEngine;
using UnityEngine.UI;

public class ClarifaiTrainScript : MonoBehaviour {

    ClarifaiLibrary _clarifai;
    Text _conceptName;

	// Use this for initialization
	void Start () {
        _conceptName = transform.FindChild("ContentName").GetComponent<Text>();
        _clarifai = GameObject.Find("Clarifai").GetComponent<ClarifaiLibrary>().Clarifai;
	}

    /// <summary>
    /// 
    /// </summary>
    public void TrainImage()
    {
        string[] _string = new string[1];
        _string[0] = _conceptName.text;

        _clarifai.AddImage(_string, CameraController.Texture);

        GameObject.Find("ExtTable").GetComponent<ExtendTableController>().buttonPressed();
    }
}
