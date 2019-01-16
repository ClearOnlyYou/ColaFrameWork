﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UGUI事件的处理容器
/// </summary>
public interface IUGUIEventHandler
{
    /// <summary>
    /// 注册UIEventListener
    /// </summary>
    /// <param name="obj"></param>
    void AttachListener(GameObject obj);

    void AddEventaHandler(Selectable st);

}

public class UGUIEventListener : MonoBehaviour,
                                  IMoveHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler, IPointerClickHandler,
        ISubmitHandler, ICancelHandler
{
    void Start()
    {

    }

    protected Selectable mCurSelectableComponent;

    public Selectable CurSelectable
    {
        get
        {
            if (mCurSelectableComponent != null)
                return mCurSelectableComponent;
            else
                return null;
        }
        set
        {
            mCurSelectableComponent = value;
        }
    }

    /// <summary>
    /// EventListener有一个统一的uihandler来接收处理回调
    /// </summary>
    public IUGUIEventHandler uiHandler;

    public delegate void UIEventHandler(GameObject obj);
    public delegate void UIDragEventHandlerDetail(GameObject obj, Vector2 deltaPos, Vector2 curToucPosition);
    public UIEventHandler onClick;
    public UIEventHandler onDown;
    public UIEventHandler onUp;
    public UIDragEventHandlerDetail onDownDetail;
    public UIDragEventHandlerDetail onUpDetail;
    public UIDragEventHandlerDetail onDrag;
    public UIEventHandler onExit;
    public UIEventHandler onDrop;
    public UIEventHandler onSelect;
    public UIEventHandler onDeSelect;
    public UIEventHandler onMove;
    public UIDragEventHandlerDetail onBeginDrag;
    public UIDragEventHandlerDetail onEndDrag;
    public UIEventHandler onEnter;
    public UIEventHandler onSubmit;
    public UIEventHandler onScroll;
    public UIEventHandler onCancel;
    public UIEventHandler onUpdateSelected;
    public UIEventHandler onInitializePotentialDrag;
    /// <summary>
    /// 新增触发事件回调，参数为触发的UI事件名称，比如onClick,onBoolValueChange,onSubmit等等
    /// </summary>
    public Action<string> onEvent;

    //统一检查是否需要屏蔽点击事件
    protected bool CheckNeedHideEvent()
    {
        if (this.mCurSelectableComponent == null || !this.mCurSelectableComponent.interactable
            || !this.mCurSelectableComponent.enabled)
        {
            return true;
        }
        return false;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onClick");
        }
        if (this.onClick != null)
        {
            this.onClick(gameObject);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onDown");
            this.onEvent("onDownDetail");
        }
        if (this.onDown != null)
        {
            this.onDown(gameObject);
        }
        if (this.onDownDetail != null)
        {
            this.onDownDetail(gameObject, eventData.delta, eventData.position);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onUp");
            this.onEvent("onUpDetail");
        }
        if (this.onUp != null)
        {
            this.onUp(gameObject);
        }
        if (this.onUpDetail != null)
        {
            this.onUpDetail(gameObject, eventData.delta, eventData.position);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onExit");
        }
        if (this.onExit != null)
        {
            this.onExit(gameObject);
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onSelect");
        }
        if (this.onSelect != null)
        {
            this.onSelect(gameObject);
        }
    }



    public virtual void OnMove(AxisEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onMove");
        }
        if (this.onMove != null)
        {
            this.onMove(gameObject);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onEnter");
        }
        if (this.onEnter != null)
        {
            this.onEnter(gameObject);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onSubmit");
        }
        if (this.onSubmit != null)
        {
            this.onSubmit(gameObject);
        }
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onCancel");
        }
        if (this.onCancel != null)
        {
            this.onCancel(gameObject);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (CheckNeedHideEvent())
        {
            return;
        }
        if (null != onEvent)
        {
            this.onEvent("onDeSelect");
        }
        if (this.onDeSelect != null)
        {
            this.onDeSelect(gameObject);
        }
    }


    //public static UGUIEventListener GetEventListenner(GameObject obj)
    //{
    //    UGUIEventListener listenner = obj.GetComponent<UGUIEventListener>();
    //    if (listenner == null)
    //    {
    //        listenner = obj.AddComponent<UGUIEventListener>();
    //    }
    //    return listenner;
    //}
}
