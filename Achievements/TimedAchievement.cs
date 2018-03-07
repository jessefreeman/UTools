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
    [Serializable]
    public class TimedAchievement : GenericAchievement
    {
        public enum TimeConditons
        {
            None,
            Before,
            After
        }

        public bool maintainValue;

        protected float time = -1;

        public TimeConditons timeCondition = TimeConditons.None;
        public Vector2 timeRange = new Vector2(-1, -1);

        public string timeToken = "${time}";

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Timed Achievement")]
        public static void CreateTimedAchievement()
        {
            AssetUtil.CreateAsset<TimedAchievement>();
        }
#endif

        protected override float CalculateCompletion()
        {
            var percent = 0f; //
            var currentDuration = statsManager.GetStatValue(AchievementManager.achievementTimeStatName);


            switch (timeCondition)
            {
                case TimeConditons.Before:
                    var basePercent = base.CalculateCompletion();
                    // see how much time is left

                    if (currentDuration > time && basePercent != 1)
                    {
                        Failed();
                        return 0;
                    }

                    percent = basePercent;
                    break;
                case TimeConditons.After:

                    var statValue = statsManager.GetStatValue(statName);

                    if (value != statValue)
                    {
                        Failed();
                        return 0;
                    }

                    percent = currentDuration / time;

                    break;
            }

            return percent;
        }

        public override string GetMessage()
        {
            var sb = new StringBuilder(base.GetMessage());
            if (time > 0)
                sb.Replace(timeToken, time.ToString());


            return sb.ToString();
        }

        protected override T CloneAchievement<T>()
        {
            var clone = base.CloneAchievement<TimedAchievement>();
            clone.timeToken = timeToken;
            clone.timeRange = timeRange;
            clone.timeCondition = timeCondition;
            //clone.maintainValue = maintainValue;

            return (T) (GenericAchievement) clone;
        }

        public override void Activate()
        {
            base.Activate();
            time = (int) Random.Range(timeRange.x, timeRange.y);
            //timeElapsed = 0;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TimedAchievement))]
    public class TimedAchievementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
#endif
}