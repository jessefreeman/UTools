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

using System.Collections.Generic;
using UnityEngine;

namespace jessefreeman.utools
{
    public class ObjectPool : MonoBehaviour
    {
        private readonly List<RecycleGameObject> poolInstances = new List<RecycleGameObject>();

        public RecycleGameObject prefab;

        public RecycleGameObject NextObject(Vector3 pos, bool restart = true)
        {
            foreach (var instance in poolInstances)
                if (instance.gameObject.activeSelf != true)
                {
                    instance.transform.position = pos;
                    if (restart)
                        instance.Restart();
                    return instance;
                }

            return CreateInstance(pos, restart);
        }

        private RecycleGameObject CreateInstance(Vector3 pos, bool restart = true)
        {
            var clone = Instantiate(prefab);
            clone.transform.position = pos;
            clone.transform.parent = transform;
            clone.name += poolInstances.Count.ToString();
            if (restart)
                clone.Restart();
            poolInstances.Add(clone);

            return clone;
        }

        public int TotalActiveInstances()
        {
            return GetActiveInsatnces().Count;
        }

        public List<RecycleGameObject> GetActiveInsatnces()
        {
            var instances = new List<RecycleGameObject>();

            foreach (var instance in poolInstances)
                if (instance.gameObject.activeSelf)
                    instances.Add(instance);

            return instances;
        }

        public void DestroyActiveInstances()
        {
            foreach (var instance in poolInstances)
                if (instance.gameObject.activeSelf)
                    GameObjectUtil.Destroy(instance.gameObject);
        }
    }
}