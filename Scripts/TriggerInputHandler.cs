using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TriggerInputHandler : MonoBehaviour
{
    public GameObject parent;
    private bool isInTriggerZone = false;
    private Collider currentCollider;
    private TMP_Text textComponent;
    private myfiles parentMyFiles;
    public bool hold;

    void Start()
    {
        // Cache the myfiles component
        parentMyFiles = parent.GetComponent<myfiles>();
    }

    void Update()
    {
        

        if (!isInTriggerZone) return;

        if (Input.GetKeyDown(KeyCode.Space) && !hold)
        //For the VR controller you need something like: if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.3f && !hold)
        {
            HandleInput();
            hold = true;   //added hold because VR controller doesn't have buttondown event. We wait 1 sec before accepting another press.
            Invoke("release", 1); 
        }
    }

    void release()
    {
        hold = false;
    }

    private void HandleInput()
    {
        if (currentCollider == null) return;

        Transform parentTransform = currentCollider.transform.parent;
        string parentName = parentTransform.name;

        if (parentName == "Down") parentMyFiles.Scroll(1);
        else if (parentName == "Up") parentMyFiles.Scroll(-1);
        else if (parentName == "Previous") parentMyFiles.DoPrevious();
        else if (parentName == "DownFiles") parentMyFiles.ScrollFiles(1);
        else if (parentName == "UpFiles") parentMyFiles.ScrollFiles(-1);
        else HandleFileSelection(parentTransform, parentName);
    }

    private void HandleFileSelection(Transform parentTransform, string parentName)
    {
        if (textComponent != null && !parentName.Contains("file"))
        {
            Transform childTransform = textComponent.gameObject.transform.GetChild(0);
            parentMyFiles.selected = childTransform.name;
            parentMyFiles.GetFolders();
        }
        else
        {
            //The actual file selection:
            parentMyFiles.selected = currentCollider.name;
            string numberPart = parentName.Replace(" file", "");
            if (int.TryParse(numberPart, out int number)) parentMyFiles.fIndex = number;
            //execute...
            parentMyFiles.execute();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        isInTriggerZone = true;
        currentCollider = other;
        textComponent = other.transform.parent.GetComponent<TMP_Text>();

        if (currentCollider.name != "Collider")
        {
            textComponent.color = Color.red;
        }
        else
        {
            other.transform.parent.GetComponent<Image>().color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != currentCollider) return;

        ResetColor();

        isInTriggerZone = false;
        currentCollider = null;
    }

    private void ResetColor()
    {
        if (currentCollider == null) return;

        if (currentCollider.name != "Collider" && !currentCollider.transform.parent.name.Contains("file"))
        {
            textComponent.color = Color.white;
        }
        else if (currentCollider.transform.parent.name.Contains("file"))
        {
            textComponent.color = Color.green;
        }
        else
        {
            currentCollider.transform.parent.GetComponent<Image>().color = Color.white;
        }
    }
}
