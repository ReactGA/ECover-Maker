using UnityEngine;   
using UnityEngine.UI;   
using System.IO;
using UnityEngine.Networking;
using System.Collections;
//using SFB;
using SimpleFileBrowser;
public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] Material bookfrontMat,laptopfrontMat;
    [SerializeField] TriggerCapture triggerCapture;
    [SerializeField]Toggle TransparentToggle,ReflectionToogle;
    [SerializeField]GameObject ColorPickerBtn,GrdColorPickerBtn,ReflectionPlane,ColorPickerImage;
    [SerializeField]GameObject Book,Laptop;
    Texture defTex,defTex1;
    [SerializeField]CameraOrbit CamOrbt;
    private void OnEnable()
    {
        defTex = bookfrontMat.mainTexture;
        defTex1 = laptopfrontMat.mainTexture;
    }
    void Start() {
        InitializeSFB();
    }
    public void PickDesign()
    {
        StartCoroutine( ShowLoadDialogCoroutine() );
    }
    void UpdateTexture(Texture tex ){
        bookfrontMat.mainTexture = tex;
        laptopfrontMat.mainTexture = tex;
    }
    public Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }

    bool ShowBook = true;
    public void SwitchObj(){
        ShowBook = !ShowBook;
        Book.SetActive(ShowBook);
        Laptop.SetActive(!ShowBook);
    }
    public void SaveImage()
    {
        CamOrbt.enabled = false;
        StartCoroutine( ShowSaveDialogCoroutine() );
    }
    public void TransparentToggleClick(){
        if(TransparentToggle.isOn){
            Camera.main.backgroundColor = new Color(0.3396226f,0.3396226f,0.3396226f);
        }
    }
    public void UseReflectionToggleClick(){
        if(TransparentToggle.isOn){TransparentToggle.isOn = false;}
        TransparentToggle.interactable = (!ReflectionToogle.isOn);
        ReflectionPlane.SetActive(ReflectionToogle.isOn);
        GrdColorPickerBtn.SetActive(ReflectionToogle.isOn);
    }
    public void PickColor(){
        ColorPickerImage.SetActive(true);
    }
    public void SelectingColor(){
        ColorPicker.Create(Camera.main.backgroundColor,"", SelectColor,FinishedPick);
    }
    void SelectColor(Color currentColor){
        Camera.main.backgroundColor = currentColor;
    }
    void FinishedPick(Color FinishedColor){}
    public void SelectingColor1(){
        var Rend = ReflectionPlane.GetComponent<Renderer>().material;
        ColorPicker.Create(Rend.color,"", SelectColor1,FinishedPick1);
    }
    void SelectColor1(Color currentColor){
        ReflectionPlane.GetComponent<Renderer>().material.color = currentColor;
    }
    void FinishedPick1(Color FinishedColor){}
    private void OnDestroy()
    {
        bookfrontMat.mainTexture = defTex;
        laptopfrontMat.mainTexture = defTex1;
    }
    void InitializeSFB(){

		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ), new FileBrowser.Filter( "Text Files", ".txt", ".pdf" ) );

		FileBrowser.SetDefaultFilter( ".jpg" );

		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );

		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
    }
    IEnumerator ShowLoadDialogCoroutine()
	{
		
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load" );

		if( FileBrowser.Success )
		{
            StartCoroutine(GetTexture(FileBrowser.Result[0]));
            yield break;

			byte[] bytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result[0] );

			string destinationPath = Path.Combine( Application.persistentDataPath, FileBrowserHelpers.GetFilename( FileBrowser.Result[0] ) );
			FileBrowserHelpers.CopyFile( FileBrowser.Result[0], destinationPath );
            
		}
	}
    IEnumerator ShowSaveDialogCoroutine(){
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders);

		if( FileBrowser.Success )
		{
            triggerCapture.CaptureAndSave(FileBrowser.Result[0], "Object",TransparentToggle.isOn);
            yield break;
        }
    }
    IEnumerator GetTexture(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError
             || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                UpdateTexture(tex);
            }
        }
    }
}
