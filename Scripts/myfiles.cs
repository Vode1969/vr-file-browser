using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;

// VR fileBrowser by Citor3 Entertainment Studios.
// You need to setup the XR before using this tool. Testing can be done by rotating the offsetOBJ in 2D.
// 
// Description in brief:
//
// The offsetOBJ, or a copy of it should go under the VR camera or VR controller. You can move and rotate it freely
// to get best match with your controller. The pointer follows the offsetOBJ with delay, so it smooths out all jerky movements.
// Just remember to set the "parent" of the smoothbeam script to the offset object under your VR rig.
// This script filters out all system files and shows you only the files with defined extension.
// At the bottom of this script you can do whatever with the chosen file.
//
// The script "TriggerInputHandler" requires setting up the XR controller, example line provided.
//
// Under the hood:
// We create copies of the TMPro objects when reading folders or files. Each TMPro object has a child collider object
// interacting with the 'pointer' object. We store full path to the collider name, index to the TMPro name and current file/folder is shown as a TMPro text.

public class myfiles : MonoBehaviour
{
    public string filterFiles;
    public string selected; //will be the full path to your file
    public int fIndex; //index of the filelist.

    public GameObject source;
    public GameObject filesource;
    public List<TMP_Text> MyFolders = new List<TMP_Text>();

    
    private string[] MyPath;
    public string[] usedPath;

    public GameObject down;
    public GameObject up;
    public GameObject previous;
    public TMP_Text curdir;
    public Sprite fol_Closed;
    public Sprite fol_Open;

    private string[] filesList;
    public List<TMP_Text> files = new List<TMP_Text>();
    public GameObject filedown;
    public GameObject fileup;

    public TMP_Text fileCount;
    public TMP_Text folderCount;

    private int folderIndex;
    private int fileIndex;
    private int depth;

    void Start()
    {
        filterFiles = ".txt"; //can be .mp3, .mp4 or whatever.
        MyPath = Directory.GetLogicalDrives();
        depth = 0;
        filedown.SetActive(false);
        fileup.SetActive(false);
        down.SetActive(false);
        up.SetActive(false);
        PopulateFolderList();

    }
       
    public void GetFolders()
    {
        ClearLists();

        MyPath = Directory.GetDirectories(selected);
        usedPath[depth] = selected;
        depth++;
        PopulateFolderList();
    }

    public void DoPrevious()
    {
        if (depth == 0) return;

        ClearLists();
        usedPath[depth] = "";
        depth--;
        selected = depth > 0 ? usedPath[depth - 1] : usedPath[depth];
        MyPath = depth == 0 ? Directory.GetLogicalDrives() : Directory.GetDirectories(selected);
        PopulateFolderList();
    }

    public void Scroll(int direction)
    {
        ScrollList(MyFolders, direction, down, up);
    }

    public void ScrollFiles(int direction)
    {
        ScrollList(files, direction, filedown, fileup);
    }

    private void ScrollList(List<TMP_Text> textList, int direction, GameObject downObj, GameObject upObj)
    {
        int activeCount = 0;
        foreach (var text in textList)
        {
            text.transform.localPosition += new Vector3(0, 8 * direction, 0);
            bool isActive = text.transform.localPosition.y <= 2 && text.transform.localPosition.y >= -5;
            text.gameObject.SetActive(isActive);
            if (isActive) activeCount++;
        }

        upObj.SetActive(textList[0].transform.localPosition.y > 2);
        downObj.SetActive(activeCount >= 8);
        downObj.GetComponent<Image>().color = Color.white;
        upObj.GetComponent<Image>().color = Color.white;
    }

    private void PopulateFolderList()
    {
        folderIndex = 0;
        SetScrollButtonsState(false);

        foreach (var folder in MyPath)
        {
            if (ShouldSkipFolder(folder)) continue;

            var clone = Instantiate(source, transform);
            clone.name = folderIndex.ToString();
            clone.transform.localPosition += new Vector3(0, -folderIndex, 0);

            var folderText = clone.GetComponent<TMP_Text>();
            var childTransform = folderText.gameObject.transform.GetChild(0);
            childTransform.name = folder;

            folderText.text = depth > 0 ? Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) : folder;
            folderText.gameObject.SetActive(true);

            MyFolders.Add(folderText);
            folderIndex++;
            

            if (folderIndex > 8) clone.gameObject.SetActive(false);
        }

        
        if (folderIndex > 8) down.SetActive(true);
        previous.GetComponent<Image>().sprite = depth > 0 ? fol_Open : fol_Closed;
        curdir.text = depth > 0 ? Path.GetFullPath(selected).TrimEnd(Path.DirectorySeparatorChar) : "Select the Drive";

        if (depth == 0)
        {
            fileCount.text = "No " + filterFiles + " Files found.";
            folderCount.text = "Found " + folderIndex.ToString() + " drives.";
        } else folderCount.text = "Found " + folderIndex.ToString() + " subfolders.";

        if (depth > 0) GetFiles();
    }

    private bool ShouldSkipFolder(string folder)
    {
        string root = Path.GetPathRoot(folder);
        bool isRoot = string.Equals(folder, root, StringComparison.OrdinalIgnoreCase);
        FileAttributes attributes = File.GetAttributes(folder);
        return !isRoot && ((attributes & FileAttributes.System) == FileAttributes.System || (attributes & FileAttributes.Hidden) == FileAttributes.Hidden);
    }

    private void ClearLists()
    {
        ClearList(MyFolders);
        ClearList(files);

        filesList = new string[0];
        folderIndex = 0;
        fileIndex = 0;
    }

    private void ClearList(List<TMP_Text> textList)
    {
        foreach (var text in textList)
        {
            if (text != null) Destroy(text.gameObject);
        }
        textList.Clear();
    }

    private void GetFiles()
    {
        fileIndex = 0;
        SetScrollButtonsState(false, true);

        try
        {
            filesList = Directory.GetFiles(selected, "*"+filterFiles);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading directory: " + ex.Message);
        }

        for (int i = 0; i < filesList.Length; i++)
        {
            var clone = Instantiate(filesource, transform);
            clone.name = $"{i} file";
            clone.transform.localPosition += new Vector3(0, -fileIndex, 0);

            var fileText = clone.GetComponent<TMP_Text>();
            var childTransform = fileText.gameObject.transform.GetChild(0);
            childTransform.name = filesList[i];

            fileText.text = Path.GetFileName(filesList[i].TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            fileText.gameObject.SetActive(true);

            files.Add(fileText);
            fileIndex++;

            if (fileIndex > 8) clone.gameObject.SetActive(false);
        }

        if (fileIndex > 0) fileCount.text = "Found " + fileIndex.ToString() + filterFiles + " files."; else fileCount.text = "No " + filterFiles + " Files found.";
        if (fileIndex > 8) filedown.SetActive(true);
    }

    private void SetScrollButtonsState(bool state, bool isFile = false)
    {
        if (isFile)
        {
            filedown.SetActive(state);
            fileup.SetActive(state);
        }
        else
        {
            down.SetActive(state);
            up.SetActive(state);
        }
    }

    public void execute()
    {
        //You have now selected a file, so do what you want to do with it.
        //The 'selected' variable contains full path with the filename,
        //fIndex contains the index number of that file (if you want to use indexing to automatically select the next or previous file).

        Debug.Log("You selected " + selected + " at Index " + fIndex + ". Total of "+ fileIndex + " files available.");
        
    }
}
