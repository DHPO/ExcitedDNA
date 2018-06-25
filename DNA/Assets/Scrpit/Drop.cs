using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    //鼠标进入  
    public void OnPointerEnter(PointerEventData eventData)
    {
        //有拖拽物，drop区域变色，否则return  
        GameObject dragobj = eventData.pointerDrag;

        if (dragobj == null) return;

        GetComponent<Image>().color = Color.green;
    }

    //鼠标离开 重置颜色white  
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
    }

    //drop 赋予拖拽物体sprite  
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragobj = eventData.pointerDrag;

        // if (dragobj == null) return;  

        GetComponent<Image>().sprite = dragobj.GetComponent<Image>().sprite;

        GetComponent<Image>().color = Color.white;
    }
}
