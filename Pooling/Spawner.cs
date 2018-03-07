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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jessefreeman.utools
{
    public class Spawner : MonoBehaviour
    {
        protected bool _active;

        private float delay;

        public Vector2 delayRange = new Vector2(1, 2);
        public List<GameObject> instances = new List<GameObject>();
        public GameObject[] prefabs;
        public bool running = true;
        public virtual bool active { get; set; }

        protected virtual void Start()
        {
            //TODO need to break this out into it's own method so it's easy to trun on and off?
            if (prefabs == null)
                return;

            // Make sure we don't start the coroutine if there is nothing to spawn
            if (prefabs.Length < 1)
                return;

            // Start spawner
            ResetDelay();
            StartCoroutine(EnemyGenerator());
        }

        private IEnumerator EnemyGenerator()
        {
            yield return new WaitForSeconds(delay);

            if (active)
            {
                GetNextInstance(); //GameObjectUtil.Instantiate(prefabs[Random.Range(0, prefabs.Length)], newTransform.position);
                ResetDelay();
            }

            StartCoroutine(EnemyGenerator());
        }

        public virtual Vector3 GetNextPosition()
        {
            return transform.position;
        }

        protected virtual int GetTotal()
        {
            return prefabs.Length - 1;
        }

        protected virtual GameObject GetNextInstance()
        {
            var prefab = GetNextPrefab();

            //Container for instance we are creating
            var instance = GameObjectUtil.Instantiate(prefab, GetNextPosition());
            //instance.gameObject.transform.position = transform.position;
            // instance.gameObject.tag = spawnedObjectTag;
            instances.Add(instance);

            return instance;
        }

        protected virtual GameObject GetNextPrefab()
        {
            return prefabs[Random.Range(0, GetTotal())];
        }

        protected void ResetDelay()
        {
            // use this for a delay range
            delay = Random.Range(delayRange.x, delayRange.y);
        }

        public void DestoryInstances()
        {
//        Debug.Log(name+" spawner is destroying " + instances.Count + " " + instances);
            foreach (var instance in instances) GameObjectUtil.Destroy(instance);
        }
    }
}