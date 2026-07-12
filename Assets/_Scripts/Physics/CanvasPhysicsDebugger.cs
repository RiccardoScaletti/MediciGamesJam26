using UnityEngine;

public class CanvasPhysicsDebugger : MonoBehaviour
{

    [SerializeField] private Vector3 debugDirection;
    [SerializeField] private float debugMagnitude;
    [SerializeField] private ForceMode debugForceMode;
    private PlayerPhysics playerPhysicsScr;

    public void callPhysicsMethod()
    {
        playerPhysicsScr.ApplyForce(debugDirection, debugMagnitude, debugForceMode);
    }

    public void StorePlayerPhysicScript(PlayerPhysics scr)
    {
        playerPhysicsScr = scr;
    }
}
