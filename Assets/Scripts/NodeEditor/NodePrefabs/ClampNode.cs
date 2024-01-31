using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class ClampNode : NodeAttributes
    {
        public override string Description => "Clamps all values into the given range.";

        [Header("Elements")]
        public TMP_InputField Min;
        public TMP_InputField Max;

        protected override void OnInit()
        {
            Min.onValueChanged.AddListener((x) => Node.RecalculateOutput());
            Max.onValueChanged.AddListener((x) => Node.RecalculateOutput());
        }


        public override GradientNoise GetOutput()
        {
            if (Node.Inputs[0] == null) return null;
            if (Min.text == "") return null;
            if (Max.text == "") return null;

            float min = float.Parse(Min.text);
            float max = float.Parse(Max.text);

            return new ModularGradientNoise(Node.Inputs, new ClampOperation(min, max));
        }
    }
}
