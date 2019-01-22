﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFrame
{
    /// <summary>
    /// 对象池类(单独存储某一类物件的对象池)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 存储堆栈
        /// </summary>
        private readonly Stack<T> stack;
        /// <summary>
        /// 创建对象的方法(外部传入)
        /// </summary>
        private Func<T> createAction;
        /// <summary>
        /// 获取对象的方法(外部传入)
        /// </summary>
        private Action<T> getAction;
        /// <summary>
        /// 回收对象的方法(外部传入)
        /// </summary>
        private Action<T> releaseAction;

        public ObjectPool(Func<T> createAction, Action<T> getAction, Action<T> relaseAction)
        {
            stack = new Stack<T>();
            this.createAction = createAction;
            this.getAction = getAction;
            this.releaseAction = relaseAction;
        }

        /// <summary>
        /// 取得一个对象池中的物件，如果没有就新创建一个
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T obj = null;
            if (stack.Count == 0)
            {
                //执行创建操作
                if (null != createAction)
                {
                    obj = createAction();
                }
                else
                {
                    Debug.LogWarning(string.Format("类型:{0}的创建方法不能为空！", typeof(T)));
                }
            }
            else
            {
                obj = stack.Pop();
            }

            if (null == obj)
            {
                Debug.LogWarning(string.Format("获取类型:{0}的物体失败！请检查!", typeof(T)));
            }
            if (null != createAction)
            {
                getAction(obj);
            }
            return obj;
        }

        /// <summary>
        /// 回收/释放一个物体
        /// </summary>
        /// <param name="obj"></param>
        public void Release(T obj)
        {
            if (null == obj)
            {
                Debug.LogWarning(string.Format("回收的{0}类型的物件为空！", typeof(T)));
                return;
            }

            if (null != releaseAction)
            {
                releaseAction(obj);
            }
            stack.Push(obj);
        }

        /// <summary>
        /// 清理当前的对象池
        /// </summary>
        public void Clear()
        {
            stack.Clear();
        }
    }


    /// <summary>
    /// 快速-轻量级的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuickObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly Action<T> m_ActionOnGet;
        private readonly Action<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public QuickObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("release error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
    }
}
