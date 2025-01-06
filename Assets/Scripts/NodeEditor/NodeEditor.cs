using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateNoiseLibrary
{
    public class NodeEditor : MonoBehaviour
    {
        private const float CONNECTION_LINE_WIDTH = 0.1f;
        private const float CAMERA_DRAG_SPEED = 0.95f;
        private const float CAMERA_ZOOM_SPEED = 2f;

        public Camera Camera;

        [Header("Elements")]
        public UI_PresetSelection PresetSelection;
        public GameObject AddNodeButtonsContainer;

        [Header("Prefabs")]
        public Sprite EmptySprite;
        public Node SourceNoiseNodePrefab;
        public Button AddNodeButtonPrefab;
        public List<Node> ModificationNodes;

        // Creating connections
        public List<Connection> Connections = new List<Connection>();
        private bool IsConnecting;
        private Connection CurrentConnection;

        // Camera Drag
        public bool IsDraggingCamera;
        private Vector3 StartDragCameraPos;
        private Vector3 StartDragMousePos;

        // Presets
        private const float DEFAULT_SCALE = 0.05f;
        public static List<GradientNoise> GradientNoisePresets;

        private void Start()
        {
            GradientNoisePresets = new List<GradientNoise>()
            {
                new PerlinNoise(DEFAULT_SCALE),
                new WhiteNoise(DEFAULT_SCALE),
                new RidgedMultifractalNoise(DEFAULT_SCALE),
                new SkewedPerlinNoise(DEFAULT_SCALE),
            };

            PresetSelection.Init(this);

            foreach (Node n in ModificationNodes)
            {
                Button b = Instantiate(AddNodeButtonPrefab, AddNodeButtonsContainer.transform);
                b.GetComponentInChildren<TextMeshProUGUI>().text = n.NodeDisplayName;
                b.onClick.AddListener(() => AddModificationNode(n));
            }
        }

        private void Update()
        {
            if (IsConnecting)
            {
                CurrentConnection.Line.positionCount = 2;
                CurrentConnection.Line.SetPosition(0, CurrentConnection.From.OutputButton.transform.position);
                CurrentConnection.Line.SetPosition(1, Camera.ScreenToWorldPoint(Input.mousePosition));

                if (Input.GetMouseButtonDown(1)) // rightclick - discard
                {
                    GameObject.Destroy(CurrentConnection.gameObject);
                    CurrentConnection = null;
                    IsConnecting = false;
                }
            }

            // Camera Drag
            if (Input.GetMouseButtonDown(0) && !IsConnecting)
            {
                IsDraggingCamera = true;
                StartDragCameraPos = Camera.transform.position;
                StartDragMousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (IsDraggingCamera)
            {
                Vector3 mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 offset = (mousePos - StartDragMousePos) * CAMERA_DRAG_SPEED;
                Camera.transform.position = new Vector3(StartDragCameraPos.x - offset.x, StartDragCameraPos.y - offset.y, Camera.transform.position.z);
            }
            if (Input.GetMouseButtonUp(0) && IsDraggingCamera)
            {
                IsDraggingCamera = false;
            }

            // Camera zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float newZoom = Camera.orthographicSize - scroll * CAMERA_ZOOM_SPEED;
                Camera.main.orthographicSize = newZoom;
            }
        }

        public void AddSourceNoiseNode(GradientNoise noise)
        {
            Node newNode = Instantiate(SourceNoiseNodePrefab);
            newNode.transform.position = new Vector2(Camera.transform.position.x + 4f, Camera.transform.position.y);
            newNode.Init(this);
            newNode.SetInput(0, noise);
        }
        public void AddModificationNode(Node prefab)
        {
            Node newNode = Instantiate(prefab);
            newNode.transform.position = new Vector2(Camera.transform.position.x + 4f, Camera.transform.position.y);
            newNode.Init(this);
        }
        public void RemoveNode(Node node)
        {
            // Remove input connections
            foreach(Connection fromCon in node.InputConnections)
                if (fromCon != null) RemoveConnection(fromCon);

            // Remove output connections
            while (node.OutputConnections.Count > 0) RemoveConnection(node.OutputConnections[0]);

            // Remove node
            GameObject.Destroy(node.gameObject);
        }

        public void StartConnecting(Node from)
        {
            if (IsConnecting) return;

            IsConnecting = true;

            GameObject newConObj = new GameObject("Connection");
            CurrentConnection = newConObj.AddComponent<Connection>();
            CurrentConnection.From = from;
            CurrentConnection.Line = newConObj.AddComponent<LineRenderer>();
            CurrentConnection.Line.material = new Material(Shader.Find("Sprites/Default"));
            CurrentConnection.Line.startWidth = CONNECTION_LINE_WIDTH;
            CurrentConnection.Line.endWidth = CONNECTION_LINE_WIDTH;
            CurrentConnection.Line.startColor = Color.white;
            CurrentConnection.Line.endColor = Color.white;
        }
        public void ConnectTo(Node to, int inputIndex)
        {
            if (!IsConnecting) return;
            if (to == CurrentConnection.From) return; // can't connect to self
            if (CurrentConnection.From.GetAllPreviousNodes().Contains(to)) return; // cycle protection

            // Connect
            CurrentConnection.To = to;
            CurrentConnection.ToInputIndex = inputIndex;

            // Remove existing input connection
            if (to.InputConnections[CurrentConnection.ToInputIndex] != null) RemoveConnection(to.InputConnections[CurrentConnection.ToInputIndex]);

            // Update display
            CurrentConnection.UpdateLine();

            // Register
            CurrentConnection.From.OutputConnections.Add(CurrentConnection);
            CurrentConnection.To.InputConnections[CurrentConnection.ToInputIndex] = CurrentConnection;
            Connections.Add(CurrentConnection);

            // Recalculate
            CurrentConnection.To.SetInput(CurrentConnection.ToInputIndex, CurrentConnection.From.Output);

            // Stop connecting
            CurrentConnection = null;
            IsConnecting = false;
        }
        public void RemoveConnection(Connection con)
        {
            // Recalculate next nodes
            con.To.InputConnections[con.ToInputIndex] = null;
            con.To.SetInput(con.ToInputIndex, null);

            // Deregister
            con.From.OutputConnections.Remove(con);
            Connections.Remove(con);

            // Destroy
            GameObject.Destroy(con.gameObject);
        }

        public void OnNodeMove(Node node)
        {
            foreach (Connection inputCon in node.InputConnections)
                if (inputCon != null) inputCon.UpdateLine();
            foreach (Connection outputCon in node.OutputConnections) outputCon.UpdateLine();
        }

        #region Code generator

        /// <summary>
        /// Copies to clipboard the C# code needed to recreate the final noise (the node's Output),
        /// which is presumably a ModularGradientNoise or a base noise.
        /// </summary>
        /// <param name="node">The node whose output we want to replicate in code.</param>
        public void CopyOutputNoiseToClipboard(Node node)
        {
            if (node == null || node.Output == null)
            {
                Debug.LogWarning("Node or Node.Output is null - cannot copy code.");
                return;
            }

            // We'll accumulate lines in this list
            List<string> lines = new List<string>();

            // A dictionary to avoid generating code for the same GradientNoise multiple times
            Dictionary<GradientNoise, string> visited = new Dictionary<GradientNoise, string>();

            int counter = 0;

            // Recursively generate code for node.Output.
            // This returns the variable name that holds the final noise.
            string finalVar = GenerateNoiseCode(node.Output, lines, visited, ref counter);

            // Mark final usage
            lines.Add("// This is the final noise reference:");
            lines.Add($"GradientNoise finalNoise = {finalVar};");

            // Join all lines into one text block
            string code = string.Join("\n", lines);

#if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.systemCopyBuffer = code;
#else
            // If not in the Editor, you can try:
            GUIUtility.systemCopyBuffer = code;
#endif

            Debug.Log($"The following code has been copied to the clipboard:\n\n{code}");
        }

        /// <summary>
        /// Recursively creates lines of code that replicate the given GradientNoise object.
        /// Returns the variable name associated with this object.
        /// </summary>
        private string GenerateNoiseCode(
            GradientNoise noise,
            List<string> lines,
            Dictionary<GradientNoise, string> visited,
            ref int counter)
        {
            // If we've already generated code for this instance, just re-use
            if (visited.ContainsKey(noise))
            {
                return visited[noise];
            }

            // We'll name variables like noise0, noise1, noise2,...
            string varName = $"noise{counter}";
            counter++;

            // --- Base noises ---
            if (noise is WhiteNoise white)
            {
                lines.Add($"var {varName} = new WhiteNoise({noise.Scale}f);");
                visited[noise] = varName;
                return varName;
            }
            else if (noise is PerlinNoise perlin)
            {
                lines.Add($"var {varName} = new PerlinNoise({noise.Scale}f);");
                visited[noise] = varName;
                return varName;
            }
            else if (noise is RidgedMultifractalNoise ridge)
            {
                lines.Add($"var {varName} = new RidgedMultifractalNoise({noise.Scale}f);");
                visited[noise] = varName;
                return varName;
            }
            else if (noise is SkewedPerlinNoise skewedPerlin)
            {
                lines.Add($"var {varName} = new SkewedPerlinNoise({noise.Scale}f);");
                visited[noise] = varName;
                return varName;
            }
            // --- ModularNoise ---
            else if (noise is ModularGradientNoise modular)
            {
                // Generate code for each *used* input, based on the operation's NumInputs.
                List<string> inputVarNames = new List<string>();

                // Make sure we have a valid operation:
                NoiseOperation op = modular.Operation;
                int numNeeded = (op != null) ? op.NumInputs : 0;

                for (int i = 0; i < numNeeded; i++)
                {
                    if (modular.Inputs == null || i >= modular.Inputs.Length || modular.Inputs[i] == null)
                    {
                        // This input is missing or null - handle gracefully
                        lines.Add($"// Operation expects input {i}, but it's missing or null. Using 'null'.");
                        inputVarNames.Add("null");
                    }
                    else
                    {
                        // Recursively generate code for the i-th input
                        string inVarName = GenerateNoiseCode(modular.Inputs[i], lines, visited, ref counter);
                        inputVarNames.Add(inVarName);
                    }
                }

                // Generate code for the operation
                string operationCode = GenerateOperationCode(op, $"{varName}_op", lines);

                // Now assemble the new ModularGradientNoise
                lines.Add($"var {varName} = new ModularGradientNoise(");
                lines.Add($"    new GradientNoise[] {{ {string.Join(", ", inputVarNames)} }},");
                lines.Add($"    {operationCode}");
                lines.Add($");");

                visited[noise] = varName;
                return varName;
            }

            // Unknown fallback
            lines.Add($"// Unknown GradientNoise type: {noise.GetType().Name}");
            lines.Add($"GradientNoise {varName} = null;  // Fallback");
            visited[noise] = varName;
            return varName;
        }

        /// <summary>
        /// Returns a string that instantiates the public Operation fields for known operations.
        /// e.g. "new AddOperation()", "new BlendOperation(0.5f)", etc.
        /// </summary>
        private string GenerateOperationCode(NoiseOperation op, string opVarName, List<string> lines)
        {
            if (op == null)
                return "null // (no operation)";

            // Check each known operation
            if (op is AddOperation)
            {
                return "new AddOperation()";
            }
            else if (op is BlendOperation blend)
            {
                // "public float BlendRatio;"
                return $"new BlendOperation({blend.BlendRatio}f)";
            }
            else if (op is ClampOperation clamp)
            {
                return $"new ClampOperation({clamp.Min}f, {clamp.Max}f)";
            }
            else if (op is CutoffOperation cutoff)
            {
                return $"new CutoffOperation({cutoff.Min}f, {cutoff.Max}f)";
            }
            else if (op is InverseOperation)
            {
                return "new InverseOperation()";
            }
            else if (op is LayerOperation layer)
            {
                string layerOpVar = $"{opVarName}_layer";
                lines.Add($"var {layerOpVar} = new LayerOperation(" +
                          $"{layer.NumOctaves}, {layer.Lacunarity}f, {layer.Persistence}f);");
                return layerOpVar;
            }
            else if (op is MaskOperation)
            {
                return "new MaskOperation()";
            }
            else if (op is MultiplyOperation)
            {
                return "new MultiplyOperation()";
            }
            else if (op is ScaleOperation scaleOp)
            {
                return $"new ScaleOperation({scaleOp.Scale}f)";
            }

            // Fallback
            return "// Unknown NoiseOperation\nnull";
        }

    #endregion
    }
}
