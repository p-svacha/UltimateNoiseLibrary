using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplyNode : NodeAttributes
{
    public override string Description => "Multiplies the 2 input nodes.";

    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;
        if (Node.Inputs[1] == null) return null;

        return new ModularGradientNoise(Node.Inputs, new MultiplyOperation());
    }
}
