using UnityEngine;


public enum physicDirectionType { defined,world}
[CreateAssetMenu(fileName = "SO_PhysicsInteraction", menuName = "Scriptable Objects/SO_PhysicsInteraction")]
public class SO_PhysicsInteraction : ScriptableObject
{
    public physicInteractions physicInteraction;
    public physicDirectionType physicDirectionType;
    public float magnitude;
    public Vector3 distance;
    public ForceMode forceMode; 
}
