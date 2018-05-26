using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Tool : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //生成的icon  
    private GameObject draged_icon;

    public void OnBeginDrag(PointerEventData eventData)
    {

        //设置icon属性，添加组件  
        Debug.Log(this.name);
 
        draged_icon = new GameObject("icon");
        draged_icon.transform.localScale /= 5;

        draged_icon.transform.SetParent(GameObject.Find("Canvas").transform, false);
        
        draged_icon.AddComponent<RectTransform>();
        draged_icon.AddComponent<Image>();
        draged_icon.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        

        //让图标不执行事件检测，防止icon妨碍后面的event system  
        CanvasGroup group = draged_icon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //在RectTransform下 实现鼠标 物体的跟随效果  
        Vector3 worldpos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draged_icon.GetComponent<RectTransform>(), eventData.position, Camera.main, out worldpos))
        {
            draged_icon.transform.position = worldpos;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        if (draged_icon != null)
        {
            GameObject tmp = new GameObject();
            switch (this.name)
            {
                case "A":
                    tmp = (GameObject)Resources.Load("Prefabs/A");
                    break;
                case "C":
                    tmp = (GameObject)Resources.Load("Prefabs/C");
                    break;
                case "G":
                    tmp = (GameObject)Resources.Load("Prefabs/G");
                    break;
                case "T":
                    tmp = (GameObject)Resources.Load("Prefabs/T");
                    break;
                default:
                    break;
            }
            
            
            Instantiate(tmp);
            tmp.transform.position = draged_icon.transform.position;
            
            Destroy(draged_icon.gameObject);
        }
        
    }
}