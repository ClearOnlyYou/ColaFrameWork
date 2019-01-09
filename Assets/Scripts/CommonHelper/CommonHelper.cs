﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Def = GloablDefine;

/// <summary>
/// 通用工具类（C#端，lua端专门有Common_Util.cs供导出使用）
/// 提供一些常用功能的接口
/// </summary>
public static class CommonHelper
{
    #region 变量定义区
    /// <summary>
    /// 中文数字在Language表中索引的base位置
    /// </summary>
    private static int chineseNumIndex = 1;

    /// <summary>
    /// 资源可读写路径
    /// </summary>
    private static string assetPath = "";

#if UNITY_STANDALONE
    public static string osDir = "Win";
#elif UNITY_ANDROID
    public static string osDir = "Android";
#elif UNITY_IPHONE
    public static string osDir = "iOS";        
#else
    public static string osDir = "";        
#endif
    #endregion

    /// <summary>
    /// 通过ID获取国际化文字
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetText(int id)
    {
        return I18NHelper.GetInstance().GetI18NText(id);
    }

    /// <summary>
    /// 给按钮添加点击事件(以后可以往这里添加点击声音)
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void AddBtnMsg(GameObject go, Action<GameObject> callback)
    {
        if (null != go)
        {
            Button button = go.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    callback(go);
                });
            }
            else
            {
                Debug.LogWarning("该按钮没有挂载button组件！");
            }
        }
        else
        {
            Debug.LogWarning("ButtonObj为空！");
        }
    }

    /// <summary>
    /// 删除一个按钮的点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    public static void RemoveBtnMsg(GameObject go, Action<GameObject> callback)
    {
        if (null != go)
        {
            Button button = go.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.RemoveAllListeners();
            }
            else
            {
                Debug.LogWarning("该按钮没有挂载button组件！");
            }
        }
        else
        {
            Debug.LogWarning("ButtonObj为空！");
        }
    }

    /// <summary>
    /// 根据id实例化一个Prefab
    /// </summary>
    /// <param name="id"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateGoByID(int id, GameObject parent)
    {
        ResourceMgr resourceMgr = ResourceMgr.GetInstance();
        GameObject prefab = null;

        if (null != resourceMgr)
        {
            prefab = resourceMgr.GetResourceById<GameObject>(id);
            if (null == prefab) return null;
            GameObject go = GameObject.Instantiate(prefab);
            if (null == go) return null;
            go.name = prefab.name;
            if (null != parent)
            {
                go.transform.SetParent(parent.transform, false);
            }
            go.transform.localScale = prefab.transform.localScale;
            go.transform.localPosition = prefab.transform.localPosition;
            go.transform.localRotation = prefab.transform.localRotation;
            return go;
        }
        else
        {
            Debug.LogWarning("检查资源管理器！");
            return null;
        }
    }

    /// <summary>
    /// 根据一个预制实例化一个Prefab
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateGoByPrefab(GameObject prefab, GameObject parent)
    {
        if (null == prefab) return null;
        GameObject obj = GameObject.Instantiate(prefab);
        if (null == obj) return null;
        obj.name = prefab.name;
        if (null != parent)
        {
            obj.transform.SetParent(parent.transform, false);
        }
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.transform.localRotation = prefab.transform.localRotation;
        obj.transform.localScale = prefab.transform.localScale;
        return obj;
    }

    /// <summary>
    /// 给物体添加一个单一组件
    /// </summary>
    /// <typeparam name="T"></typeparam>组件的类型
    /// <param name="go"></param>要添加组件的物体
    /// <returns></returns>
    public static T AddSingleComponent<T>(this GameObject go) where T : Component
    {
        if (null != go)
        {
            T component = go.GetComponent<T>();
            if (null == component)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }
        Debug.LogWarning("要添加组件的物体为空！");
        return null;
    }

    /// <summary>
    /// 获取某个物体下对应名字的子物体上的某个类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>组件的类型
    /// <param name="go"></param>父物体
    /// <param name="name"></param>子物体的名称
    /// <returns></returns>
    public static T GetComponentByName<T>(this GameObject go, string name) where T : Component
    {
        T[] components = go.GetComponentsInChildren<T>(true);
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null && components[i].name == name)
                {
                    return components[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取某个物体下子物体上某种类型的所有组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T[] GetComponentsByName<T>(this GameObject go) where T : Component
    {
        T[] components = go.GetComponentsInChildren<T>(true);

        return components;
    }

    /// <summary>
    /// 获取某个物体下对应的名字的所有子物体
    /// </summary>
    /// <param name="go"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static List<GameObject> GetGameObjectsByName(this GameObject go, string childName)
    {
        List<GameObject> list = new List<GameObject>();
        Transform[] objChildren = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < objChildren.Length; ++i)
        {
            if ((objChildren[i].name.Contains(childName)))
            {
                list.Add(objChildren[i].gameObject);
            }
        }
        return list;
    }

    /// <summary>
    /// 根据路径查找物体上的某个类型的组件(可以是自己本身/子物体/父节点)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="childPath"></param>
    /// <returns></returns>
    public static T GetComponentByPath<T>(this GameObject obj, string childPath) where T : Component
    {
        GameObject childObj = obj.FindChildByPath(childPath);
        if (null == childObj)
        {
            return null;
        }
        T component = childObj.GetComponent<T>();
        if (null == component)
        {
            Debug.LogWarning(String.Format("没有在路径:{0}上找到组件:{1}!", childPath, typeof(T)));
            return null;
        }
        return component;
    }

    /// <summary>
    /// 获取资源管理器
    /// </summary>
    /// <returns></returns>
    public static ResourceMgr GetResourceMgr()
    {
        return ResourceMgr.GetInstance();
    }

    /// <summary>
    /// 获取系统管理器
    /// </summary>
    /// <returns></returns>
    public static ModuleMgr GetModuleMgr()
    {
        return GameManager.GetInstance().GetModuleMgr();
    }

    /// <summary>
    /// 获取UI管理器
    /// </summary>
    /// <returns></returns>
    public static UIMgr GetUIMgr()
    {
        return GameManager.GetInstance().GetUIMgr();
    }

    /// <summary>
    /// 设置一个Image组件的Sprite为某个图集中的图片
    /// </summary>
    /// <param name="atlasID"></param>图集的资源ID
    /// <param name="image"></param>要设置的Image组件
    /// <param name="spriteName"></param>图集中对应的Sprite的名称
    /// <param name="keepNativeSize"></param>是否保持原始尺寸
    public static void SetImageSpriteFromAtlas(int atlasID, Image image, string spriteName, bool keepNativeSize)
    {
        if (null == image)
        {
            Debug.LogWarning("需要指定一个image");
            return;
        }
        if (string.IsNullOrEmpty(spriteName))
        {
            Debug.LogWarning("需要指定一个SpriteName");
            return;
        }

        if (image.overrideSprite && image.overrideSprite.name == spriteName)
        {
            return;
        }
        GameObject atlasObj = ResourceMgr.GetInstance().GetResourceById<GameObject>(atlasID);
        if (null != atlasObj)
        {
            SpriteAsset spriteAsset = atlasObj.GetComponent<SpriteAsset>();
            if (null != spriteAsset)
            {
                Sprite sprite = spriteAsset.GetSpriteByName(spriteName);
                if (null != sprite)
                {
                    image.overrideSprite = sprite;
                    if (keepNativeSize)
                    {
                        image.SetNativeSize();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置一个RawImage的Texture
    /// </summary>
    /// <param name="rawImage"></param>
    /// <param name="resID"></param>
    /// <param name="keepNativeSize"></param>
    public static void SetRawImage(RawImage rawImage, int resID, bool keepNativeSize)
    {
        if (null == rawImage)
        {
            Debug.LogWarning("需要指定RawImage");
            return;
        }

        Texture2D texture2D = ResourceMgr.GetInstance().GetResourceById<Texture2D>(resID);
        if (null != texture2D)
        {
            rawImage.texture = texture2D;
            if (keepNativeSize)
            {
                rawImage.SetNativeSize();
            }
        }
    }

    /// <summary>
    /// 设置一个Image是否变灰
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isGray"></param>
    public static void SetImageGray(Image image, bool isGray)
    {
        if (null == image)
        {
            Debug.LogWarning("需要指定一个Image");
            return;
        }
        image.color = isGray ? Def.ColorGray : Def.ColorWhite;
    }

    /// <summary>
    /// RawImage置灰
    /// </summary>
    /// <param name="rawImage"></param>
    /// <param name="isGray"></param>
    public static void SetRawImageGray(RawImage rawImage, bool isGray)
    {
        if (null == rawImage)
        {
            Debug.LogWarning("需要指定一个RawImage");
            return;
        }
        if (isGray)
        {
            Material grayMat = ResourceMgr.GetInstance().GetResourceById<Material>(300001);
            rawImage.material = grayMat;
            rawImage.color = Def.ColorBlack;
        }
        else
        {
            rawImage.material = null;
            rawImage.color = Def.ColorWhite;
        }
    }

    /// <summary>
    /// 获取当前运行的设备平台信息
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Device And Sysinfo:\r\n");
        stringBuilder.Append(string.Format("DeviceModel: {0} \r\n", SystemInfo.deviceModel));
        stringBuilder.Append(string.Format("DeviceName: {0} \r\n", SystemInfo.deviceName));
        stringBuilder.Append(string.Format("DeviceType: {0} \r\n", SystemInfo.deviceType));
        stringBuilder.Append(string.Format("BatteryLevel: {0} \r\n", SystemInfo.batteryLevel));
        stringBuilder.Append(string.Format("DeviceUniqueIdentifier: {0} \r\n", SystemInfo.deviceUniqueIdentifier));
        stringBuilder.Append(string.Format("SystemMemorySize: {0} \r\n", SystemInfo.systemMemorySize));
        stringBuilder.Append(string.Format("OperatingSystem: {0} \r\n", SystemInfo.operatingSystem));
        stringBuilder.Append(string.Format("GraphicsDeviceID: {0} \r\n", SystemInfo.graphicsDeviceID));
        stringBuilder.Append(string.Format("GraphicsDeviceName: {0} \r\n", SystemInfo.graphicsDeviceName));
        stringBuilder.Append(string.Format("GraphicsDeviceType: {0} \r\n", SystemInfo.graphicsDeviceType));
        stringBuilder.Append(string.Format("GraphicsDeviceVendor: {0} \r\n", SystemInfo.graphicsDeviceVendor));
        stringBuilder.Append(string.Format("GraphicsDeviceVendorID: {0} \r\n", SystemInfo.graphicsDeviceVendorID));
        stringBuilder.Append(string.Format("GraphicsDeviceVersion: {0} \r\n", SystemInfo.graphicsDeviceVersion));
        stringBuilder.Append(string.Format("GraphicsMemorySize: {0} \r\n", SystemInfo.graphicsMemorySize));
        stringBuilder.Append(string.Format("GraphicsMultiThreaded: {0} \r\n", SystemInfo.graphicsMultiThreaded));
        stringBuilder.Append(string.Format("SupportedRenderTargetCount: {0} \r\n", SystemInfo.supportedRenderTargetCount));
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获取设备的电量
    /// </summary>
    /// <returns></returns>
    public static float GetBatteryLevel()
    {
        if (Application.isMobilePlatform)
        {
            return SystemInfo.batteryLevel;
        }

        return 1;
    }

    /// <summary>
    /// 获取设备的电池状态
    /// </summary>
    /// <returns></returns>
    public static BatteryStatus GetBatteryStatus()
    {
        return SystemInfo.batteryStatus;
    }

    /// <summary>
    /// 获取设备网络的状况
    /// </summary>
    /// <returns></returns>
    public static NetworkReachability GetNetworkStatus()
    {
        return Application.internetReachability;
    }

    /// <summary>
    /// 检测一个功能模块是否开启
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="isTips"></param>
    /// <returns></returns>
    public static CheckFuncResult CheckFuncOpen(string funcName, bool isTips)
    {
        //todo:做些检查工作
        CheckFuncResult result = CheckFuncResult.True;
        if (CheckFuncResult.True != result && isTips)
        {
            switch (result)
            {
                case CheckFuncResult.False:
                    Debug.LogWarning(string.Format("功能未开启{0}", funcName));
                    break;
                case CheckFuncResult.LevelLimit:
                    Debug.LogWarning(string.Format("等级限制不能开启{0}", funcName));
                    break;
                case CheckFuncResult.TimeLimit:
                    Debug.LogWarning(string.Format("时间限制不能开启{0}", funcName));
                    break;
                case CheckFuncResult.None:
                    Debug.LogWarning(string.Format("未知原因不能开启{0}", funcName));
                    break;
                default:
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// 将世界坐标转化UGUI坐标
    /// </summary>
    /// <param name="gameCamera"></param>
    /// <param name="canvas"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector2 WorldToUIPoint(Camera gameCamera, Canvas canvas, Vector3 worldPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            gameCamera.WorldToScreenPoint(worldPos), canvas.worldCamera, out pos);
        return pos;
    }

    /// <summary>
    /// 获取资源路径(可读写)
    /// </summary>
    /// <returns></returns>
    public static string GetAssetPath()
    {
        if (string.IsNullOrEmpty(assetPath))
        {
#if UNITY_IPHONE
            assetPath = string.Format("{0}/{1}", Application.temporaryCachePath, osDir);
#else
            assetPath = string.Format("{0}/{1}", Application.persistentDataPath, osDir); 
#endif
        }
        return assetPath;
    }

    /// <summary>
    /// 判断一个string数组中是否包含某个string
    /// </summary>
    /// <param name="key"></param>
    /// <param name="strList"></param>
    /// <returns></returns>
    public static bool IsArrayContainString(string key, params string[] strList)
    {
        if (null == key || null == strList)
        {
            return false;
        }
        for (int i = 0; i < strList.Length; i++)
        {
            if (strList[i].Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取主机IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetHostIp()
    {
        String url = "http://hijoyusers.joymeng.com:8100/test/getNameByOtherIp";
        string IP = "未获取到外网ip";
        try
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = Encoding.Default;
            string str = client.DownloadString(url);
            client.Dispose();

            if (!str.Equals("")) IP = str;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        Debug.Log("get host ip :" + IP);
        return IP;
    }

    /// <summary>
    /// 获取当前位置距离地面的高度
    /// </summary>
    /// <param name="vPos"></param>
    /// <param name="fRadius"></param>
    /// <returns></returns>
    public static float GetTerrainHeight(Vector3 vPos, float fRadius = 0)
    {
        var orign = vPos;
        orign.y += 300;
        float maxDistance = vPos.y + 400;
        RaycastHit hitInfo;
        bool bRet = (fRadius == 0) ? Physics.Raycast(orign, Vector3.down, out hitInfo, maxDistance, LayerMask.GetMask("Terrain")) : Physics.SphereCast(orign, fRadius, Vector3.down, out hitInfo, orign.y + 50, LayerMask.GetMask("Terrain"));
        return bRet ? hitInfo.point.y : 0;
    }

    /// <summary>
    /// 获取导航点距离地面的高度
    /// </summary>
    /// <param name="vPos"></param>
    /// <returns></returns>
    public static float GetNavMeshHeight(Vector3 vPos)
    {
        return GetTerrainHeight(vPos);
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="coroutine"></param>
    public static void StartCoroutine(IEnumerator coroutine)
    {
        GameLauncher.Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// 启动一个协程
    /// </summary>
    /// <param name="methodName"></param>
    public static void StartCoroutine(string methodName)
    {
        GameLauncher.Instance.StartCoroutine(methodName);
    }

    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="methodName"></param>
    public static void StopCoroutine(string methodName)
    {
        GameLauncher.Instance.StopCoroutine(methodName);
    }

    /// <summary>
    /// 停止所有的协程
    /// </summary>
    public static void StopAllCoroutines()
    {
        GameLauncher.Instance.StopAllCoroutines();
    }

    /// <summary>
    /// UI文字组件的打字机效果(不支持换行)
    /// </summary>
    /// <param name="textCommponent"></param>
    /// <param name="content"></param>
    /// <param name="delta"></param>
    /// <param name="onComplete"></param>
    public static void TextTyperEffect(Text textCommponent, string content, float delta, Action onComplete)
    {
        if (null != textCommponent && !string.IsNullOrEmpty(content))
        {
            textCommponent.text = "";
            int length = 1;
            int timerID = -1;

            Action killTimer = () =>
            {
                if (-1 != timerID)
                {
                    TimeHelper.KillTimer(timerID);
                }
            };

            timerID = TimeHelper.SetRepeatTimer(() =>
            {
                var subContent = content.Substring(0, length);
                textCommponent.text = subContent;
                length++;
                if (length > content.Length)
                {
                    killTimer();
                    if (null != onComplete)
                    {
                        onComplete();
                    }
                }
            }, delta);

        }
    }

    /// <summary>
    /// 阿拉伯数字转为汉字(支持0-9)
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GetChineseNumber(int number)
    {
        if (number >= 0 && number <= 9)
        {
            return I18NHelper.GetText(chineseNumIndex + number);
        }
        Debug.LogWarning(string.Format("输入的数字{0}不在0~9范围内！", number));
        return "";
    }

    /// <summary>
    /// 检查本地文件是否存在,如果目录不存在则创建目录
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckLocalFileExist(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }
        string dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            return false;
        }

        if (File.Exists(filePath))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 资源清理和垃圾回收
    /// </summary>
    public static void ResourcesClearAndGC()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    /// <summary>
    /// 切割一张Texure大图上面的一部分，作为Sprite精灵返回
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Sprite SliceTextureToSprite(Texture2D texture2D,float x,float y,float width,float height)
    {
        if (null != texture2D)
        {
            if (x + width > texture2D.width)
            {
                width = texture2D.width - x;
                Debug.LogWarning("the width is larger then texture2D width!");
            }
            if (y + height > texture2D.height)
            {
                height = texture2D.height - y;
                Debug.LogWarning("the height is larger then texture2D height!");
            }

            Sprite sprite = Sprite.Create(texture2D,new Rect(x,y,width,height),new Vector2(0.5f,0.5f));
            return sprite;
        }
        Debug.LogWarning("Texture2D 不能为空！");
        return null;
    }

}