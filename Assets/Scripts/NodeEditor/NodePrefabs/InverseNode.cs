using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseNode : NodeAttributes
{
    public override string Description => "Inverses the input.";

    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;

        return new ModularGradientNoise(Node.Inputs, new InverseOperation());
    }
}
