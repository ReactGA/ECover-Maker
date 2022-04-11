using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField]float ScrollSpeed = 2,RotateSpeed = 2;
    [SerializeField]Transform WorldCenter;
    float LastX,LastY;

    void Update()
    {
        //Camera.main.transform.Translate(Camera.main.transform.forward * Input.mouseScrollDelta * ScrollSpeed * Time.deltaTime);
        if(Input.mouseScrollDelta.y != 0){
            Debug.Log(Input.mouseScrollDelta.y);
            Camera.main.transform.Translate(Camera.main.transform.forward* Input.mouseScrollDelta.y*ScrollSpeed * Time.deltaTime);
        }
        
        if(Input.GetMouseButton(1)){
            var NormX = Mathf.Clamp(Input.mousePosition.x - LastX,-1,1);
            var NormY = Mathf.Clamp(Input.mousePosition.y - LastY,-1,1);
            if(NormX != 0){
               Camera.main.transform.RotateAround(WorldCenter.position,Vector3.up,NormX* RotateSpeed); 
            }
            /* else{
               Camera.main.transform.RotateAround(WorldCenter.position,Vector3.right,NormY* RotateSpeed); 
            }  */
            LastX = Input.mousePosition.x;
            LastY = Input.mousePosition.y;
        }
    }
}
