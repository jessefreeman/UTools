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
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace jessefreeman.utools
{
    public class TransitionManager : MonoBehaviour
    {
        public float delay = 1f;
        private bool loadLock;
        public float lockDelay;
        private float lockDelayElapsed;
        public string scene;
        public Sounds sound;
        public SoundManager soundManager;
        public bool triggerByAnyKeyPress = true;

        public TweenLayer[] tweenLayers;

        private void Start()
        {
            soundManager = GameObjectUtil.GetSingleton<SoundManager>();
            lockDelayElapsed = 0;
        }

        private void Update()
        {
            if (!triggerByAnyKeyPress)
                return;

            lockDelayElapsed += Time.deltaTime;

            if (Input.anyKey && !loadLock && lockDelayElapsed > lockDelay) OnTransitionOut();
        }

        public void OnTransitionOut()
        {
            StartCoroutine(TransitionOut());


            if (sound != Sounds.None && soundManager != null)
            {
                soundManager.FadeActiveSounds(1, 0, 2);
                soundManager.PlayClip((int) sound);
            }
        }

        public void LoadScene()
        {
            GameObjectUtil.CleanupSingletons();
            GameObjectUtil.ClearPools();
            Application.LoadLevel(scene);
        }

        private IEnumerator TransitionOut()
        {
            loadLock = true;

            foreach (var layer in tweenLayers) layer.TweenOut();

            yield return new WaitForSeconds(delay);

            LoadScene();
        }
    }
#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenLayer))]
    public class TweenLayerEditor : Editor
    {
        private TweenLayer layer;

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            layer.autoRun = EditorGUILayout.Toggle("Auto Start", layer.autoRun);
            layer.repeat = EditorGUILayout.Toggle("Repeate", layer.repeat);

            var rectTansform = layer.GetComponent<RectTransform>();

            var pos = rectTansform.anchoredPosition;

            EditorGUILayout.LabelField("Start Properties");

            // Set Start Position
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tween Duration");
            layer.startTime = EditorGUILayout.FloatField(layer.startTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start Delay");
            layer.startDelay = EditorGUILayout.FloatField(layer.startDelay);
            GUILayout.EndHorizontal();

            layer.start = EditorGUILayout.Vector3Field("Start Position", layer.start);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move To Start"))
            {
                rectTansform.anchoredPosition = new Vector2(layer.start.x, layer.start.y);
                ;
            }

            if (GUILayout.Button("Set Start")) layer.start = new Vector3(pos.x, pos.y, layer.transform.position.z);

            GUILayout.EndHorizontal();

            layer.tween = (LeanTweenType) EditorGUILayout.EnumPopup(layer.tween);

            EditorGUILayout.LabelField("End Properties");

            // Set End Position
            layer.end = EditorGUILayout.Vector3Field("End Position", layer.end);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move To End"))
            {
                Debug.Log("end position " + layer.end);
                rectTansform.anchoredPosition = new Vector2(layer.end.x, layer.end.y);
            }

            if (GUILayout.Button("Set End")) layer.end = new Vector3(pos.x, pos.y, layer.transform.position.z);

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset All"))
            {
                layer.start = new Vector2(0, 0);
                layer.end = new Vector2(0, 0);
            }

            GUILayout.EndVertical();
        }

        private void OnEnable()
        {
            layer = target as TweenLayer;
        }
    }

#endif
}