using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class SourceNoiseNode : NodeAttributes
    {
        public GameObject RandomizeSeedButton;
        public override string Description => "Input noise from the presets.";

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
