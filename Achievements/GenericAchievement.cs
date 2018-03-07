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
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace jessefreeman.utools
{
    public interface IAchievement
    {
        string GetName();
        string GetMessage();
        float GetCompletionPercent();
        void Activate();
        bool IsCompleted();
        float GetTimeElapsed();
        string FormatTimeElapsed();
    }

    [Serializable]
    public class GenericAchievement : ScriptableObject, IAchievement
    {
        public delegate void OnComplete();

        public delegate void OnFail();

        public bool autoRestartOnFail = true;
        protected bool completed;
        public Sprite completedIcon;
        public string completedMessage = "YOUR MESSAGE HERE";
        public Sprite defaultIcon;
        public Sprite failedIcon;

        public string message = "YOUR MESSAGE HERE";
        public bool resetValueOnActivate;
        public int rewardValue = 100;
        protected float startValue;
        public string statName;
        protected StatsManager statsManager;
        public string statsNameToken = "${statName}";
        public string timeElapsedToken = "${timeElapsed}";
        protected float value = 1;
        public Vector2 valueRange = new Vector2(1, 2);
        public string valueToken = "${value}";

        //TODO need a bonus or reward

        public float GetCompletionPercent()
        {
            if (completed)
                return 1;

            var percent = CalculateCompletion();
            if (percent >= 1 && !completed)
                Completed();

            return percent;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual string GetMessage()
        {
            var sb = new StringBuilder(completed ? completedMessage : message);

            sb.Replace(statsNameToken, statName);
            sb.Replace(valueToken, ((int) value).ToString());
            sb.Replace(timeElapsedToken, FormatTimeElapsed());

            return sb.ToString();
        }

        public virtual void Activate()
        {
            // TODO may need to have a seporate restart method for time ones
            value = (int) Random.Range(valueRange.x, valueRange.y);

            if (statsManager == null)
                statsManager = GameObjectUtil.GetSingleton<StatsManager>();

            if (resetValueOnActivate)
                statsManager.ResetStat(statName);

            startValue = statsManager.GetStatValue(statName);
        }

        public bool IsCompleted()
        {
            return completed;
        }

        public float GetTimeElapsed()
        {
            if (statsManager == null)
                return 0;

            return statsManager.GetStatValue(AchievementManager.achievementTimeStatName);
        }

        public string FormatTimeElapsed()
        {
            //TODO go back and fix this
            return string.Format("{0:00}", GetTimeElapsed());
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Generic Achievement")]
        public static void CreateGenericAchievement()
        {
            AssetUtil.CreateAsset<GenericAchievement>();
        }
#endif
        public event OnComplete CompleteCallback;
        public event OnFail FailCallback;

        protected virtual float CalculateCompletion()
        {
            // if not maintaing value then the value itself is the percent
            var currentValue = statsManager.GetStatValue(statName) - startValue;
            var percent = currentValue / value;

            if (percent >= 1) percent = 1;

            //TODO if this is timed and past time, reset achievement

            return percent;
        }

        public virtual IAchievement Clone()
        {
            return CloneAchievement<GenericAchievement>();
        }

        //TODO look into making a generic
        protected virtual T CloneAchievement<T>() where T : GenericAchievement
        {
            var clone = CreateInstance<T>();
            clone.name = name;
            clone.message = message;
            clone.completedMessage = completedMessage;
            clone.valueToken = valueToken;
            clone.defaultIcon = defaultIcon;
            clone.completedIcon = completedIcon;
            clone.failedIcon = failedIcon;
            clone.resetValueOnActivate = resetValueOnActivate;
            clone.statsNameToken = statsNameToken;

            clone.statName = statName;
            clone.valueRange = valueRange;

            return clone;
        }

        public void Completed()
        {
            completed = true;
            if (CompleteCallback != null)
                CompleteCallback();

            statsManager = null;
        }

        public void Failed()
        {
            if (FailCallback != null)
                FailCallback();

            if (autoRestartOnFail) Activate();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GenericAchievement))]
    public class GenericAchievementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
#endif
}