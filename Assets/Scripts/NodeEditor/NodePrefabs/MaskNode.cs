using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskNode : NodeAttributes
{
    public override string Description => "Adds two inputs together based on a mask. Input 1 will be set onto the are of the mask and Input 2 on the inversed area of the mask.";

    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;
        if (Node.Inputs[1] == null) return null;
        if (Node.Inputs[2] == null) return null;

        return new ModularGradientNoise(Node.Inputs, new MaskOperation());
    }
}
