using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーンをまたいでも共有できるサービスロケーター。
/// すべての管理クラス（DeckManagerなど）をここに登録して取得する。
/// </summary>
public static class Locator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// サービス登録。例: Locator.Register<DeckManager>(this);
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
            services[type] = service;
        else
            services.Add(type, service);
    }

    /// <summary>
    /// 登録済みサービスの取得。例: Locator.Get<DeckManager>();
    /// </summary>
    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
            return service as T;

        Debug.LogError($"Service not found: {type.Name}");
        return null;
    }

    /// <summary>
    /// すべてのサービス登録を削除（必要に応じて）
    /// </summary>
    public static void Clear()
    {
        services.Clear();
    }
}
