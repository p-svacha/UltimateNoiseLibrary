using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class BlendNode : NodeAttributes
    {
        public override string Description => "Blends 2 inputs together. The blend value (0-1) determines the strength of the first input.";

        [Header("Elements")]
        public TMP_InputField BlendValue;

        protected override void OnInit()
        {
            BlendValue.onValueChanged.AddListener((x) => Node.RecalculateOutput());
        }

        public override GradientNoise GetOutput()
        {
            if (Node.Inputs[0] == null) return null;
            if (Node.Inputs[1] == null) return null;
            if (BlendValue.text == "") return null;

            float blendValue = float.Parse(BlendValue.text);

            return new ModularGradientNoise(Node.Inputs, new BlendOperation(blendValue));
        }
    }
}
