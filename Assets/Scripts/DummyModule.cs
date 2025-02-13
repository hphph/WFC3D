using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DummyModule : MonoBehaviour
{
    [System.Serializable]
    public abstract class AbstractConnector
    {
        public int ConnectionId;
        public DummyModule[] ExcludedDummyModules;
    }

    [System.Serializable]
    public class VerticalConnector: AbstractConnector
    {
        public enum RotationState 
        { 
            A, B, C, D, Invariant 
        }
        public RotationState Rotation;

        public override string ToString()
        {
            string result = this.ConnectionId.ToString();
            switch(Rotation)
            {
                case RotationState.A:
                    result += "A";
                    break;
                case RotationState.B:
                    result += "B";
                    break;
                case RotationState.C:
                    result += "C";
                    break;
                case RotationState.D:
                    result += "D";
                    break;
                case RotationState.Invariant:
                    result += "I";
                    break;
            }
            return result;
        }
    }

    [System.Serializable]
    public class HorizontalConnector: AbstractConnector
    {
        public bool Symmetric;
        public bool Flipped;
        public override string ToString()
        {
            string result = this.ConnectionId.ToString();
            result += Symmetric ? "S" : Flipped ? "F" : "";
            return result;
        }
    }

    public HorizontalConnector Left;
    public HorizontalConnector Forward;
    public VerticalConnector Up;
    public HorizontalConnector Right;
    public HorizontalConnector Back;
    public VerticalConnector Down;
    public float Probability;
    public string[] Tags;

    public AbstractConnector[] ModuleConnectors 
    {
        get 
        {
            return new AbstractConnector[] {Left, Forward, Up, Right, Back, Down};
        }
    }

    public HorizontalConnector[] HorizontalConnectors
    {
        get
        {
            return new HorizontalConnector[] {Left, Forward, Right, Back};
        }
    }

    public VerticalConnector[] VerticalConnectors
    {
        get
        {
            return new VerticalConnector[] {Up, Down};
        }
    }

    public AbstractConnector OppositeConnector(int connectorIndex)
    {
        if(connectorIndex > 6) throw new System.Exception("Connector index out of bounds");
        else return ModuleConnectors[WFCTools.OppositeConnectorIndex(connectorIndex)];
    }

    void PrintDirectionLabels(Color textColor)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = textColor;
        Handles.Label(transform.position + transform.up, Up.ToString(), style);
        Handles.Label(transform.position + transform.forward, Forward.ToString(), style);
        Handles.Label(transform.position + transform.right, Right.ToString(), style);
        Handles.Label(transform.position + -transform.forward, Back.ToString(), style);
        Handles.Label(transform.position + -transform.right, Left.ToString(), style);
        Handles.Label(transform.position + -transform.up, Down.ToString(), style);
    }

    void OnDrawGizmosSelected()
    {
        if(PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            PrintDirectionLabels(Color.black);
        }
    }

    void OnDrawGizmos()
    {
        if(PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            PrintDirectionLabels(Color.white);
        }
    }
}
