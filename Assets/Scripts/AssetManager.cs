using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}

public class AssetManager : SingletonManager<AssetManager>
{

    private Dictionary<string, AsyncOperationHandle> _dicAsyncOperationHandle = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<string, Object> _dicAssets = new Dictionary<string, Object>();

    private int _nProcess = 0;

    /// <summary>
    /// �޸� �ε�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="strKey"></param>
    /// <param name="onActCallBack"></param>
    public void LoadAssetAsync<T>(string strKey, Action<T> onActCallBack = null) where T : UnityEngine.Object
    {
        //�̹� �ε��.
        if (_dicAssets.TryGetValue(strKey, out Object asset))
        {
            onActCallBack?.Invoke(asset as T);
            return;
        }

        //�ε���.
        if (_dicAsyncOperationHandle.ContainsKey(strKey))
        {
            _dicAsyncOperationHandle[strKey].Completed += (o) =>
            {
                onActCallBack?.Invoke(o.Result as T);
                return;
            };
        }
        //�ε�����.
        else
        {
            _nProcess = 0;
            //�񵿱� �޸� �ε�.
            _dicAsyncOperationHandle.Add(strKey, Addressables.LoadAssetAsync<T>(strKey));

            _nProcess++;
            _dicAsyncOperationHandle[strKey].Completed += (o) =>
            {
                _dicAssets.Add(strKey, o.Result as Object);
                onActCallBack?.Invoke(o.Result as T);
                _nProcess--;
            };
        }

        StartCoroutine(WaitLoadingCoroutine());
    }

    /// <summary>
    /// �޸� ����.
    /// </summary>
    /// <param name="strKey"></param>
    public void Release(string strKey)
    {
        if (!_dicAssets.TryGetValue(strKey, out Object asset))
            return;
        _dicAssets.Remove(strKey);

        if (_dicAsyncOperationHandle.TryGetValue(strKey, out AsyncOperationHandle asyncOperationHandle))
            Addressables.Release(asyncOperationHandle);
        _dicAsyncOperationHandle.Remove(strKey);
    }

    /// <summary>
    /// �޸� ����.
    /// </summary>
    public void Clear()
    {
        _dicAssets.Clear();

        foreach (var AsyncOperationHandle in _dicAsyncOperationHandle.Values)
        {
            Addressables.Release(AsyncOperationHandle);
        }
        _dicAsyncOperationHandle.Clear();
    }

    /// <summary>
    /// ������ ����.
    /// </summary>
    /// <param name="strKey">������ �̸�</param>
    /// <param name="traParent"></param>
    /// <param name="onActCallBack"></param>
    public void Instantiate(string strKey, Transform traParent, Action<GameObject> onActCallBack = null)
    {
        LoadAssetAsync<GameObject>(strKey, (prefab) =>
        {
            GameObject go = Instantiate(prefab, traParent) as GameObject;
            go.name = prefab.name;
            go.transform.localPosition = prefab.transform.position;
            onActCallBack?.Invoke(go);
        });
    }

    /// <summary>
    /// ������ ����.
    /// </summary>
    /// <param name="go"></param>
    public void Destroy(GameObject go)
    {
        Object.Destroy(go);
    }

    /// <summary>
    /// �ε� �Ϸ� �Ǳ� ������ ���.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitLoadingCoroutine()
    {
        while (_nProcess > 0)
        {
            yield return null;
        }
    }
}
