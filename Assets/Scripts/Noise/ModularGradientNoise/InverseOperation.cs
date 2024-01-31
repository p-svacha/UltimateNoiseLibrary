using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class InverseOperation : NoiseOperation
    {
        public override float DoOperation(GradientNoise[] inputs, float x, float y)
        {
            return 1f - inputs[0].GetValue(x, y);
        }
    }
}
