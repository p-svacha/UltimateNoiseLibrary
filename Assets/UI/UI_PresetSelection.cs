using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNoiseLibrary
{
    public class UI_PresetSelection : MonoBehaviour
    {
        private NodeEditor NodeEditor;

        [Header("Prefabs")]
        public GameObject NoisePresetRowPrefab;
        public UI_NoisePreset NoisePresetPrefab;

        [Header("Elements")]
        public GameObject InfiniteGradientContainer;
        public GameObject FiniteSegmentationContainer;

        // Layout values
        private const int PRESETS_PER_ROW = 4;
        private GameObject CurrentInfiniteGradientRow;

        public void Init(NodeEditor editor)
        {
            NodeEditor = editor;

            HelperFunctions.DestroyAllChildredImmediately(InfiniteGradientContainer, skip: 1);
            InsertInfiniteGradientNoisePreset(new PerlinNoise());
            InsertInfiniteGradientNoisePreset(new WhiteNoise());
            InsertInfiniteGradientNoisePreset(new RidgedMultifractalNoise());

            HelperFunctions.DestroyAllChildredImmediately(FiniteSegmentationContainer, skip: 1);
        }

        private void InsertInfiniteGradientNoisePreset(GradientNoise noise)
        {
            // Insert new row if necessary
            if (CurrentInfiniteGradientRow == null || CurrentInfiniteGradientRow.transform.childCount >= PRESETS_PER_ROW)
            {
                CurrentInfiniteGradientRow = GameObject.Instantiate(NoisePresetRowPrefab, InfiniteGradientContainer.transform);
                HelperFunctions.DestroyAllChildredImmediately(CurrentInfiniteGradientRow);
            }

            // Insert preset
            UI_NoisePreset preset = Instantiate(NoisePresetPrefab, CurrentInfiniteGradientRow.transform);
            preset.Init(NodeEditor, noise);
        }
    }
}
