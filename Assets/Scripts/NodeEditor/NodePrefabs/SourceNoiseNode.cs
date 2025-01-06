using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class SourceNoiseNode : NodeAttributes
    {
        public GameObject RandomizeSeedButton;
        public override string Description => "Input noise from the presets.";

        [Header("Elements")]
        public TMP_InputField Input;

        protected override void OnInit()
        {
            Input.onValueChanged.AddListener(OnScaleChanged);
        }

        private void OnScaleChanged(string inputValue)
        {
            if (float.TryParse(inputValue, out float value))
            {
                Node.Inputs[0].SetScale(value);
                Node.RecalculateOutput();
            }
        }

        public override void OnClick(GameObject target)
        {
            if (target == RandomizeSeedButton)
            {
                Node.Inputs[0].RandomizeSeed();
                Node.RecalculateOutput();
            }
        }
        public override GradientNoise GetOutput()
        {
            Node.Title.text = Node.Inputs[0].Name;
            return Node.Inputs[0].GetCopy();
        }
    }
}
