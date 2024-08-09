using UnityEngine;
using UnityEditor;

public class DummyModule : MonoBehaviour
{
    [System.Serializable]
    public abstract class AbstractConnector
    {
        public int ConnectionId;
    }

    [System.Serializable]
    public class VerticalConnector: AbstractConnector
    {
        public enum RotationState { A, B, C, D, Invariant }
        public RotationState rotation;

        public override string ToString()
        {
            string result = this.ConnectionId.ToString();
            switch(rotation)
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
        public bool Filpped;
        public override string ToString()
        {
            string result = this.ConnectionId.ToString();
            result += Symmetric ? "S" : Filpped ? "F" : "";
            return result;
        }
    }

    public VerticalConnector Up;
    public HorizontalConnector Forward;
    public HorizontalConnector Right;
    public HorizontalConnector Back;
    public HorizontalConnector Left;
    public VerticalConnector Down;

    public AbstractConnector[] ModuleConnectors 
    {
        get 
        {
            return new AbstractConnector[] {Up, Forward, Right, Back, Left, Down};
        }
    }

    public float Probability;

    void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, Up.ToString());
        Handles.Label(transform.position + Vector3.forward, Forward.ToString());
        Handles.Label(transform.position + Vector3.right, Right.ToString());
        Handles.Label(transform.position + Vector3.back, Back.ToString());
        Handles.Label(transform.position + Vector3.left, Left.ToString());
        Handles.Label(transform.position + Vector3.down, Down.ToString());
    }
}
