using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 静的サービスロケーター。シーンを跨いで各マネージャへアクセスするために使います。
/// 使用例:
///   Locator.Register<CardLibrary>(this);
///   var lib = Locator.Get<CardLibrary>();
/// </summary>
public static class Locator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        var t = typeof(T);
        services[t] = service;
    }

    public static T Get<T>() where T : class
    {
        var t = typeof(T);
        if (services.TryGetValue(t, out var s))
            return s as T;
        Debug.LogError($"[Locator] Service {t.Name} not found.");
        return null;
    }

    public static void Clear()
    {
        services.Clear();
    }
}
