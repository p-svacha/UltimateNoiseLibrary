using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LayerNode : NodeAttributes
{
    public override string Description => "Layers the input noise on itself.";

    [Header("Elements")]
    public TMP_InputField NumOctavesInput;
    public TMP_InputField LacunarityInput;
    public TMP_InputField PersistenceInput;

    protected override void OnInit()
    {
        NumOctavesInput.onValueChanged.AddListener((x) => Node.RecalculateOutput());
        LacunarityInput.onValueChanged.AddListener((x) => Node.RecalculateOutput());
        PersistenceInput.onValueChanged.AddListener((x) => Node.RecalculateOutput());
    }


    public override GradientNoise GetOutput()
    {
        if (Node.Inputs[0] == null) return null;
        if (NumOctavesInput.text == "") return null;
        if (LacunarityInput.text == "") return null;
        if (PersistenceInput.text == "") return null;

        int numOctaves = int.Parse(NumOctavesInput.text);
        float persistence = float.Parse(PersistenceInput.text);
        float lacunarity = float.Parse(LacunarityInput.text);

        return new ModularGradientNoise(Node.Inputs, new LayerOperation(numOctaves, lacunarity, persistence));
    }
}
