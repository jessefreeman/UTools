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

namespace jessefreeman.utools
{
    public enum PlatformInputTypes
    {
        Mouse,
        Touch,
        Controller,
        Keyboard
    }

    public class PlatformDetectionUtil : MonoBehaviour
    {
        private readonly Dictionary<string, PlatfromInputDefinition> platformOverrides =
            new Dictionary<string, PlatfromInputDefinition>();

        private void Awake()
        {
            Debug.Log("Request Platform Def\n" + GetPlatformDefinition().ToString());

            RegisterPlatforms();
        }

        private void RegisterPlatforms()
        {
            // Handle Fire TV Platforms
            var inputs = CreateDefaultInputDefinition();
            inputs[PlatformInputTypes.Touch] = false;

            var fireTV = new PlatfromInputDefinition("Amazon AFTB", inputs, DeviceType.Console);
            platformOverrides.Add(fireTV.deviceModel, fireTV);

            var fireTVStick = new PlatfromInputDefinition("Amazon AFTM", inputs, DeviceType.Console);
            platformOverrides.Add(fireTVStick.deviceModel, fireTVStick);
        }

        public PlatfromInputDefinition GetPlatformDefinition()
        {
            //TODO need to look up existing platform definitions
            var currentDeviceModel = SystemInfo.deviceModel;

            if (platformOverrides.ContainsKey(currentDeviceModel))
                return platformOverrides[currentDeviceModel];


            var inputs = CreateDefaultInputDefinition();

            var def = new PlatfromInputDefinition(SystemInfo.deviceModel, inputs);

            return def;
        }

        private static Dictionary<PlatformInputTypes, bool> CreateDefaultInputDefinition()
        {
            var inputs = new Dictionary<PlatformInputTypes, bool>();
            //TODO figure out best order for this
            inputs.Add(PlatformInputTypes.Controller, Input.GetJoystickNames().Length > 0);
            inputs.Add(PlatformInputTypes.Keyboard, true);
            inputs.Add(PlatformInputTypes.Mouse, SystemInfo.deviceType == DeviceType.Desktop);
            inputs.Add(PlatformInputTypes.Touch,
                Input.touchSupported); // TODO see if this returns correct value on fire tv
            return inputs;
        }
    }

    public class PlatfromInputDefinition
    {
        public string deviceModel;

        public DeviceType deviceType;

        public PlatformInputTypes[] inputOrder = new PlatformInputTypes[4]
        {
            PlatformInputTypes.Controller, PlatformInputTypes.Keyboard, PlatformInputTypes.Mouse,
            PlatformInputTypes.Touch
        };

        private readonly Dictionary<PlatformInputTypes, bool> inputs;

        public PlatfromInputDefinition(string deviceModel, Dictionary<PlatformInputTypes, bool> inputs,
            DeviceType deviceType = DeviceType.Unknown, PlatformInputTypes[] inputOrder = null)
        {
            this.deviceModel = deviceModel;
            this.inputs = inputs;

            if (inputOrder != null)
                this.inputOrder = inputOrder;

            this.deviceType = deviceType == DeviceType.Unknown ? SystemInfo.deviceType : deviceType;
        }

        public bool hasController
        {
            get { return TestInputType(PlatformInputTypes.Controller); }
        }

        public bool hasTouch
        {
            get { return TestInputType(PlatformInputTypes.Touch); }
        }

        public bool hasKeyboard
        {
            get { return TestInputType(PlatformInputTypes.Keyboard); }
        }

        public bool hasMouse
        {
            get { return TestInputType(PlatformInputTypes.Mouse); }
        }

        private bool TestInputType(PlatformInputTypes type)
        {
            var value = true;

            if (inputs.ContainsKey(type))
                value = inputs[type];

            return value;
        }

        public new string ToString()
        {
            var text = "\nDevice Model: " + deviceModel;
            text += "\nDevice Type: " + Enum.GetName(typeof(DeviceType), deviceType);

            for (var i = 0; i < inputOrder.Length; i++)
                text += "\nInput " + i + " - " + Enum.GetName(typeof(PlatformInputTypes), inputOrder[i]) + ": " +
                        inputs[inputOrder[i]];

            return text;
        }
    }
}