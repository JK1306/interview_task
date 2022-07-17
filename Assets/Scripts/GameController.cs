using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public string url;
    public ParseJSON jsonData;
    public GameObject searchPanel, resultPanel;
    public GameObject dataObject, parentObject;
    public Transform topLimit, bottomLimit; 
    public ScrollRect scrollRect;
    public Button searchBtn;
    public InputField inputField;
    public Sprite highlightImage;
    int MAX_VISIBLE_CONTENT_COUNT,
        index;
    Transform spawnedObject;
    RaycastHit2D raycastHit;
    string resString;
    void Start()
    {
        jsonData = new ParseJSON();
        StartCoroutine(GetData());
        MAX_VISIBLE_CONTENT_COUNT = 11;
    }

    private void OnEnable() {
        searchBtn.onClick.AddListener(SearchKeyWord);
    }

    private void OnDisable() {
        searchBtn.onClick.RemoveListener(SearchKeyWord);
    }

    IEnumerator GetData(){
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            resString = webRequest.downloadHandler.text;

            jsonData = JsonUtility.FromJson<ParseJSON>(resString);
            PopulateData();
        }
    }

    void PopulateData(){
        for(int i=0; i<jsonData.data.Length; i++){
            spawnedObject = Instantiate<Transform>(dataObject.transform, parentObject.transform);
            spawnedObject.GetChild(0).GetComponent<Text>().text = (i+1).ToString();
            spawnedObject.GetChild(1).GetComponent<Text>().text = jsonData.data[i].name;
            spawnedObject.GetChild(2).GetComponent<Text>().text = jsonData.data[i].code.ToString();
        }
    }

    string GetTopDataContent(){
        raycastHit = Physics2D.Raycast(topLimit.position, transform.TransformDirection(Vector3.forward), Mathf.Infinity);
        // Debug.DrawRay(topLimit.position, transform.TransformDirection(Vector3.forward), Color.white,Mathf.Infinity);
        if(raycastHit.collider){
            return raycastHit.collider.gameObject.transform.GetChild(0).GetComponent<Text>().text;
        }
        return null;
    }

    void SearchKeyWord(){
        index = CompareText(inputField.text);
        if(index < 0){

        }else{
            index = GetUpperLimitIndex(index);
            Debug.Log("Start Index : "+index);
            resultPanel.SetActive(true);
            searchPanel.SetActive(false);
            StartCoroutine(ScrollResult(index));
        }
    }

    int CompareText(string inputString){
        int childCount = parentObject.transform.childCount;
        string nameVal;
        Transform iterObject;
        for(int i=0; i<childCount; i++){
            iterObject = parentObject.transform.GetChild(i);
            nameVal = iterObject.GetChild(1).GetComponent<Text>().text;
            if(nameVal.ToLower() == inputString.ToLower()){
                iterObject.GetComponent<Image>().sprite = highlightImage;
                Debug.Log("Name : "+nameVal);
                Debug.Log("Index : "+i);
                Debug.Log("String Matched");
                return i;
            }
        }
        return -1;
    }

    int GetUpperLimitIndex(int matchIndex){
        int startIndex = 1;
        while(matchIndex > (startIndex + (MAX_VISIBLE_CONTENT_COUNT-1))){
            startIndex += MAX_VISIBLE_CONTENT_COUNT-1;
        }
        return startIndex;
    }

    IEnumerator ScrollResult(int index){
        yield return new WaitForSeconds(0.5f);
        float t = 0.0f;
        while(t < 1f && GetTopDataContent() != index.ToString()){
            t += Time.deltaTime/1f;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(1, 0, t);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }
}
