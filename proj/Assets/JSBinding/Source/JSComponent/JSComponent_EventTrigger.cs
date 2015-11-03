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
		callIfExist(idOnBeginDrag, eventData);
    }
    public virtual void OnCancel(BaseEventData eventData)
    {
		callIfExist(idOnCancel, eventData);
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
		callIfExist(idOnDeselect, eventData);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
		callIfExist(idOnDrag, eventData);
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
		callIfExist(idOnDrop, eventData);
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
		callIfExist(idOnEndDrag, eventData);
    }
    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
		callIfExist(idOnInitializePotentialDrag, eventData);
    }
    public virtual void OnMove(AxisEventData eventData)
    {
		callIfExist(idOnMove, eventData);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
		callIfExist(idOnPointerClick, eventData);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
		callIfExist(idOnPointerDown, eventData);
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
		callIfExist(idOnPointerEnter, eventData);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
		callIfExist(idOnPointerExit, eventData);
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        callIfExist(idOnPointerUp, eventData);
    }
    public virtual void OnScroll(PointerEventData eventData)
    {
		callIfExist(idOnScroll, eventData);
    }
    public virtual void OnSelect(BaseEventData eventData)
    {
		callIfExist(idOnSelect, eventData);
    }
    public virtual void OnSubmit(BaseEventData eventData)
    {
		callIfExist(idOnSubmit, eventData);
    }
    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
		callIfExist(idOnUpdateSelected, eventData);
    }
}
