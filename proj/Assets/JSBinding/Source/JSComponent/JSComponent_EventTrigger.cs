using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * 如果脚本继承了以下几种接口，则统一用这个替换
 * 这个只支持 Awake Start Update LateUpdate
 * 
 */

public class JSComponent_EventTrigger : JSComponent, 
    IEventSystemHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IPointerClickHandler, 
    IBeginDragHandler, 
    IInitializePotentialDragHandler, 
    IDragHandler, 
    IEndDragHandler, 
    IDropHandler, 
    IScrollHandler, 
    IUpdateSelectedHandler, 
    ISelectHandler, 
    IDeselectHandler, 
    IMoveHandler, 
    ISubmitHandler,
    ICancelHandler
{
    int idUpdate;
    int idLateUpdate;

    int idOnBeginDrag;
    int idOnCancel;
    int idOnDeselect;
    int idOnDrag;
    int idOnDrop;
    int idOnEndDrag;
    int idOnInitializePotentialDrag;
    int idOnMove;
    int idOnPointerClick;
    int idOnPointerDown;
    int idOnPointerEnter;
    int idOnPointerExit;
    int idOnPointerUp;
    int idOnScroll;
    int idOnSelect;
    int idOnSubmit;
    int idOnUpdateSelected;

    protected override void initMemberFunction()
    {
        base.initMemberFunction();
        idUpdate = JSApi.getObjFunction(jsObjID, "Update");
        idLateUpdate = JSApi.getObjFunction(jsObjID, "LateUpdate");

        idOnBeginDrag = JSApi.getObjFunction(jsObjID, "OnBeginDrag");
        idOnCancel = JSApi.getObjFunction(jsObjID, "OnCancel");
        idOnDeselect = JSApi.getObjFunction(jsObjID, "OnDeselect");
        idOnDrag = JSApi.getObjFunction(jsObjID, "OnDrag");
        idOnDrop = JSApi.getObjFunction(jsObjID, "OnDrop");
        idOnEndDrag = JSApi.getObjFunction(jsObjID, "OnEndDrag");
        idOnInitializePotentialDrag = JSApi.getObjFunction(jsObjID, "OnInitializePotentialDrag");
        idOnMove = JSApi.getObjFunction(jsObjID, "OnMove");
        idOnPointerClick = JSApi.getObjFunction(jsObjID, "OnPointerClick");
        idOnPointerDown = JSApi.getObjFunction(jsObjID, "OnPointerDown");
        idOnPointerEnter = JSApi.getObjFunction(jsObjID, "OnPointerEnter");
        idOnPointerExit = JSApi.getObjFunction(jsObjID, "OnPointerExit");
        idOnPointerUp = JSApi.getObjFunction(jsObjID, "OnPointerUp");
        idOnScroll = JSApi.getObjFunction(jsObjID, "OnScroll");
        idOnSelect = JSApi.getObjFunction(jsObjID, "OnSelect");
        idOnSubmit = JSApi.getObjFunction(jsObjID, "OnSubmit");
        idOnUpdateSelected = JSApi.getObjFunction(jsObjID, "OnUpdateSelected");
    }
    void Update()
    {
        callIfExist(idUpdate);
    }
    void LateUpdate()
    {
        callIfExist(idLateUpdate);
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        callIfExist(idOnBeginDrag);
    }
    public virtual void OnCancel(BaseEventData eventData)
    {
        callIfExist(idOnCancel);
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
        callIfExist(idOnDeselect);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        callIfExist(idOnDrag);
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
        callIfExist(idOnDrop);
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        callIfExist(idOnEndDrag);
    }
    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        callIfExist(idOnInitializePotentialDrag);
    }
    public virtual void OnMove(AxisEventData eventData)
    {
        callIfExist(idOnMove);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        callIfExist(idOnPointerClick);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        callIfExist(idOnPointerDown);
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        callIfExist(idOnPointerEnter);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        callIfExist(idOnPointerExit);
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        callIfExist(idOnPointerUp);
    }
    public virtual void OnScroll(PointerEventData eventData)
    {
        callIfExist(idOnScroll);
    }
    public virtual void OnSelect(BaseEventData eventData)
    {
        callIfExist(idOnSelect);
    }
    public virtual void OnSubmit(BaseEventData eventData)
    {
        callIfExist(idOnSubmit);
    }
    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
        callIfExist(idOnUpdateSelected);
    }
}
