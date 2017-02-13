using UnityEngine;
using UnityEngine.UI;

public class ExtendTableController : MonoBehaviour
{
    public Transform parentContent;

    bool _buttonPressed;
    float _dragSpeed = 5000f;
    GameObject _table;
    
    Text _extTableButtonText;

    // Use this for initialization
    void Start ()
    {
        _table              = transform.FindChild("Table").gameObject;
        _extTableButtonText = GameObject.Find("ExtTable_Button_Text").GetComponent<Text>();
        _buttonPressed      = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_buttonPressed)
        {
            _extTableButtonText.text = "Close";
            _table.GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(_table.GetComponent<RectTransform>().anchoredPosition,
                                                                Vector3.zero,
                                                                Time.deltaTime * _dragSpeed);
        }
        else
        {
            _extTableButtonText.text = "Open";
            _table.GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(_table.GetComponent<RectTransform>().anchoredPosition,
                                                                new Vector3(0, -800f, 0),
                                                                Time.deltaTime * _dragSpeed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void buttonPressed()
    {
        _buttonPressed = !_buttonPressed;
    }
}
