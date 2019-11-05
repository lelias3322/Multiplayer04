using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// Id of the network object
    /// </summary>
    [HideInInspector]
    public int id;

    ///<summary>
    ///Resource identifier for the spawner 10/1 - part 10
    /// </summary>
    public int resourceId;
    
    /// <summary> 
    /// client ID associated with this network object - Nov1
    /// </summary>
    public int clientId = -1;

    /// <summary>
    /// start position of this network object - Nov 1
    /// </summary>
    public Vector3 startPosition;

    #endregion

    #region Unity Callbacks

    // Start is called before the first frame update
    public virtual void Start()
    {
        // If we are not on the server and id is not set, destroy the gameobject
        if (Equals(GameServerManager.instance, null) && id == 0)
        {
            Destroy(gameObject);
        }
        else if (!Equals(GameServerManager.instance, null))
        {
            // Get the instance id of the gameobject on the server scene
            id = GetInstanceID();
            
            // Register with the server
            GameServerManager.instance.RegisterNetworkObject(this); 
 
            // Test for resource ID - 10/1 - Part 10
            if (resourceId == 0)
                throw new System.Exception(string.Format("There is no " +
                    "resource id for {0} gameObject", name));
        }
    }
    #endregion
}