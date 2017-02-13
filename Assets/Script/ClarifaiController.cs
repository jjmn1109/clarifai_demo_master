using UnityEngine;
using UnityEngine.UI;
using Clarifai;
using System.Collections;

public class ClarifaiController : MonoBehaviour
{
    public string modelName;
    public GameObject ContentHolderV1;
    public GameObject ContentHolderV2;    
    public GameObject ContentPrefab;
    public GameObject cameraView;
    public GameObject parentCanvas;
    public GameObject contentUI;

    ArrayList ContentsV1;
    ArrayList ContentsV2;
    ClarifaiLibrary _clarifai;

    /// <summary>
    /// 
    /// </summary>
    void Start ()
    {
        _clarifai = GameObject.Find("Clarifai").GetComponent<ClarifaiLibrary>().Clarifai;
        
        if (null != _clarifai)
        {
            _clarifai.OnAddImageWithTextureSuccess          += _clarifai_OnAddImageWithTextureSuccess;
            _clarifai.OnPredictCustomWithTextureSuccess     += _clarifai_OnPredictCustomWithTextureSuccess;
            _clarifai.OnPredictGeneralWithTextureSuccess    += _clarifai_OnPredictGeneralWithTextureSuccess;
            _clarifai.OnTrainModelSuccess                   += _clarifai_OnTrainModelSuccess;
            _clarifai.OnError                               += _clarifai_OnError;
        }

        ContentsV1 = new ArrayList();
        ContentsV2 = new ArrayList();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SendRequest()
    {
        StartCoroutine(SendRequestHelper());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendRequestHelper()
    {
        yield return new WaitForEndOfFrame();

        TakePhoto();

        _clarifai.PredictCustomWithTexture(modelName, CameraController.Texture);
        _clarifai.PredictGeneralWithTexture(modelName, CameraController.Texture);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clarifai_OnPredictCustomWithTextureSuccess(object sender, PublisherEventArgs e)
    {
        if (null != e.eventData.outputs)
        {
            foreach (var _output in e.eventData.outputs)
            {
                if (null != _output.data.concepts)
                {
                    foreach (var _concept in _output.data.concepts)
                    {
                        ContentsV2.Add(new Content(_concept.id, ((double)(_concept.value) * 100).ToString("0")));
                    }
                }
            }
        }

        foreach (Content content in ContentsV2)
        {
            GameObject newContent               = Instantiate(Resources.Load<GameObject>("Prefabs/Content"));
            ContentController controller        = newContent.GetComponent<ContentController>();

            controller.Name.text                = content.Name;
            controller.Prob.text                = content.Prob + "%";

            newContent.transform.SetParent(ContentHolderV2.transform);
            newContent.transform.localPosition  = Vector3.zero;
            newContent.transform.localScale     = Vector3.one;   
        }

        GameObject.Find("ResultUI").GetComponent<Canvas>().sortingOrder = 2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clarifai_OnPredictGeneralWithTextureSuccess(object sender, PublisherEventArgs e)
    {
        if (null != e.eventData.outputs)
        {
            foreach (var _output in e.eventData.outputs)
            {
                if (null != _output.data.concepts)
                {
                    foreach (var _concept in _output.data.concepts)
                    {
                        ContentsV1.Add(new Content(_concept.name, ((double)(_concept.value) * 100).ToString("0")));
                    }
                }
            }
        }

        foreach (Content content in ContentsV1)
        {
            GameObject newContent               = Instantiate(Resources.Load<GameObject>("Prefabs/Content"));
            ContentController controller        = newContent.GetComponent<ContentController>();
            
            controller.Name.text                = content.Name;
            controller.Prob.text                = content.Prob + "%";

            newContent.GetComponent<Button>().interactable = false;
            newContent.transform.SetParent(ContentHolderV1.transform);
            newContent.transform.localPosition  = Vector3.zero;
            newContent.transform.localScale     = Vector3.one;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clarifai_OnAddImageWithTextureSuccess(object sender, PublisherEventArgs e)
    {
        _clarifai.TrainModel(modelName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clarifai_OnTrainModelSuccess(object sender, PublisherEventArgs e)
    {
        GameObject.Find("ResultUI").GetComponent<Canvas>().sortingOrder = 0;

        GameObject newNoti = Instantiate(Resources.Load<GameObject>("Prefabs/Noti_Success"));
        newNoti.transform.SetParent(parentCanvas.transform);
        newNoti.transform.localPosition = Vector3.zero;
        newNoti.transform.localScale = Vector3.one;

        Destroy(newNoti, 2.0f);

        for (int i = ContentHolderV2.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentHolderV2.transform.GetChild(i).gameObject);
        }

        for (int i = ContentHolderV1.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentHolderV1.transform.GetChild(i).gameObject);
        }

        ContentsV1 = new ArrayList();
        ContentsV2 = new ArrayList();
    }

    /// <summary>
    /// 
    /// </summary>
    public void resetAddedImage()
    {
        for (int i = ContentHolderV2.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentHolderV2.transform.GetChild(i).gameObject);
        }

        for (int i = ContentHolderV1.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentHolderV1.transform.GetChild(i).gameObject);
        }

        ContentsV1 = new ArrayList();
        ContentsV2 = new ArrayList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clarifai_OnError(object sender, PublisherEventArgs e)
    {
        //To show the original string message from Clarifai
        Debug.Log(e.orgMsg);

        GameObject newNoti = Instantiate(Resources.Load<GameObject>("Prefabs/Noti_Failed"));
        newNoti.transform.SetParent(parentCanvas.transform);
        newNoti.transform.localPosition = Vector3.zero;
        newNoti.transform.localScale = Vector3.one;
        Destroy(newNoti, 2.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    public void TakePhoto()
    {
        CameraController.Texture = new Texture2D(CameraController.WebcamTexture.width, CameraController.WebcamTexture.height);
        CameraController.Texture.SetPixels(CameraController.WebcamTexture.GetPixels());
        CameraController.Texture = rotateTexture(CameraController.Texture);
        CameraController.Texture.Apply();

        GameObject.Find("Preview_Image").GetComponent<RawImage>().texture = CameraController.Texture;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public Texture2D rotateTexture(Texture2D image)
    {
        Texture2D target = new Texture2D(image.height, image.width, image.format, false);

        Color32[] pixels = image.GetPixels32(0);
        pixels = rotateTextureGrid(pixels, image.width, image.height);
        target.SetPixels32(pixels);
        target.Apply();

        return target;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="wid"></param>
    /// <param name="hi"></param>
    /// <returns></returns>
    public Color32[] rotateTextureGrid(Color32[] tex, int wid, int hi)
    {
        Color32[] ret = new Color32[wid * hi];

        for (int y = 0; y < hi; y++)
        {
            for (int x = 0; x < wid; x++)
            {
                ret[y + (wid - 1 - x) * hi] = tex[x + y * wid];
            }
        }
        return ret;
    }
}
