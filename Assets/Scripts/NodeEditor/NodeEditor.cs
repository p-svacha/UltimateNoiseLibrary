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
        private const float CAMERA_ZOOM_SPEED = 1f;

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

        private void Start()
        {
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
    }
}
