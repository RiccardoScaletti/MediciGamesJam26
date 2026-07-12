using System.Collections.Generic;
using UnityEngine;

public enum physicInteractions { Jump, }
public class PhysicsInteractionManager : MonoBehaviour
{
    public static PhysicsInteractionManager instance;
    public List<SO_PhysicsInteraction> interactionsList = new List<SO_PhysicsInteraction>();


    private void Start()
    {
        if(instance == null) { instance = this;}
        else { Destroy(this.gameObject); }
    }
}
