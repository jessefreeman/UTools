// Copyright 2015 - 2018 Jesse Freeman
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR

#endif

namespace jessefreeman.utools
{
    public class GameObjectUtil
    {
        public static string singletonContainer = "SingletonContainer";

        private static GameObject _poolCollection;

        private static readonly Dictionary<RecycleGameObject, ObjectPool>
            pools = new Dictionary<RecycleGameObject, ObjectPool>();

        private static readonly Dictionary<Type, MonoBehaviour> singletonInstances =
            new Dictionary<Type, MonoBehaviour>();

        private static GameObject poolCollection
        {
            get
            {
                if (_poolCollection == null) _poolCollection = new GameObject("PoolCollection");

                return _poolCollection;
            }
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 pos, bool restart = true)
        {
            GameObject instance = null;

            var recycleScript = prefab.GetComponent<RecycleGameObject>();
            if (recycleScript != null)
            {
                var pool = GetObjectPool(recycleScript);
                instance = pool.NextObject(pos, restart).gameObject;
                instance.transform.parent = pool.gameObject.transform;
            }
            else
            {
                instance = Object.Instantiate(prefab);
                instance.transform.position = pos;
            }

            return instance;
        }

        public static void Destroy(GameObject gameObject)
        {
            var recycleGameObject = gameObject.GetComponent<RecycleGameObject>();

            if (recycleGameObject != null)
                recycleGameObject.Shutdown();
            else
                Object.Destroy(gameObject);
        }

        public static void ClearPools()
        {
            pools.Clear();
        }

        private static ObjectPool GetObjectPool(RecycleGameObject reference)
        {
            ObjectPool pool = null;

            if (pools.ContainsKey(reference))
            {
                pool = pools[reference];
            }
            else
            {
                var poolContainer = new GameObject(reference.gameObject.name + "ObjectPool");
                pool = poolContainer.AddComponent<ObjectPool>();
                pool.transform.SetParent(poolCollection.transform);
                pool.prefab = reference;
                pools.Add(reference, pool);
            }

            return pool;
        }

        private static T CreateSingleton<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
//        Debug.Log("Creating Singleton " + type);

            if (!HasSingleton<T>())
            {
                var singletonsContainer = GameObject.Find(singletonContainer);
                if (singletonsContainer == null)
                    singletonsContainer = new GameObject(singletonContainer);

                var script = singletonsContainer.AddComponent<T>();
                singletonInstances.Add(type, script);
            }

            return singletonInstances[type] as T;
        }

        public static void RegisterSingleton<T>(MonoBehaviour instance, bool destroyDouplicate = true)
            where T : MonoBehaviour
        {
            var type = typeof(T);

            // Check to see if it will override the existing instance
            if (destroyDouplicate)
                DeleteSingleton<T>();

            // Create the instance
            if (!HasSingleton<T>())
                singletonInstances.Add(type, instance);
            else
                throw new Exception("Instance of " + type + " already exists");
        }

        public static T GetSingleton<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!HasSingleton<T>())
                CreateSingleton<T>();

            return singletonInstances[type] as T;
        }

        public static void UnregisterSingleton<T>(MonoBehaviour instance) where T : MonoBehaviour
        {
        }

        public static void DeleteSingleton<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (HasSingleton<T>()) singletonInstances.Remove(type);
        }

        public static bool HasSingleton<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            return singletonInstances.ContainsKey(type);
        }

        public static void CleanupSingletons()
        {
            singletonInstances.Clear();
        }
    }
}