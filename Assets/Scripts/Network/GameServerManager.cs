using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class GameServerManager : MonoBehaviourSingletonPersistent<GameServerManager>
{
    /// <summary>
    /// List of connected clients
    /// </summary>
    //...public List<int> clientsId;         //... this is a List object!
    public List<IClient> clients;

    /// <summary>
    /// Reference to the DrkRift server
    /// </summary>
    public XmlUnityServer serverReference;

    /// <summary>
    /// List of objects handled by the server
    /// </summary>
    public List<NetworkObject> networkObjects;      //... so is this!

    ///<summary>
    /// Last tick received from the server   from Part 11 ( 1 of 2)  Oct 11, 2019
    /// </summary>
    public int currentTick = -1;

    /// <summary>
    /// List of available start positions - Nov 1
    /// </summary>
    public List<Vector3> availableStartPositions;
    
    #region Unity Callbacks
    // Start is called before the first frame update
    private void Start()
    {
        /////////////////////////////////
        /// Properties initialization
        //...clientsId = new List<int>();    //... this is a List object!
        clients = new List<IClient>();       //.. Nov 1
        serverReference = GetComponent<XmlUnityServer>();
        availableStartPositions = new List<Vector3>();
        availableStartPositions.Add(new Vector3(-2, 1,-2));     // Front left corner
        availableStartPositions.Add(new Vector3(2, 1, -2));     // Front right corner         
        availableStartPositions.Add(new Vector3(-2, 1, 2));     // Rear left corner
        availableStartPositions.Add(new Vector3(2, 1, 2));      // Rear right corner
        ////////////////////////////////
        /// Events subscription
        serverReference.Server.ClientManager.ClientConnected += ClientConnected;
        serverReference.Server.ClientManager.ClientDisconnected += ClientDisconnected;
                
        ///////////////////////////////
        /// Load the game scene
        SceneManager.LoadScene("MainGameScene", LoadSceneMode.Additive);

        ///////////////////////////////
        /// Instantiate a second ball
        //...InstantiateResource("BouncyBall", new Vector3(2, 1, -2));   ///BAD CAUSED A CRASH!!!

        //... this is from Part 9 -- see "networkObjects" above
        networkObjects = new List<NetworkObject>();
    }

    void FixedUpdate()
    {
        currentTick++;
    }


    #endregion

    #region Server events

    /// <summary>
    /// Invoked when a client connects to the DarkRift server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientConnected(object sender, ClientConnectedEventArgs e)
    {
        //...ClientsId.Add(e.Client.ID);
        clients.Add(e.Client);
        Debug.Log("--->>(Connect) number of clients: " + serverReference.Server.ClientManager.Count);
        Debug.Log("--->>(from List) number in List: " + clients.Count);
        

        ///////////////////////////////
        /// Instantiate a ball associated with this client  Nov 1
        InstantiateResource("BouncyBall", availableStartPositions[0], e.Client);
        availableStartPositions.RemoveAt(0);

        // Send all objects to spawn
        SendAllObjectsToSpawnTo(e.Client);      //... callls a function below...
    }
    
    /// Invoked when a client disconnects from the DarkRift server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        clients.Remove(e.Client);

        // Find network object associated iwth this client - Nov 1
        NetworkObject no = networkObjects.Where(x => x.clientId == e.Client.ID).FirstOrDefault();  // Nov 1
        // add its start position to availableStartPosition - Nov 1
        availableStartPositions.Add(no.startPosition);                                             // Nov 1 
        // destroy the game object - Nov 1
        NetworkObject.Destroy(no.gameObject);                                                      // Nov 1
        // remove object from the networkObjects list - Nov 1
        networkObjects.Remove(no);
    }

    #endregion
    
    #region Implementation

    /// <summary>
    /// Use this function to add a network object that must be handled by the server
    /// </summary>
    /// <param name="pNetworkObject"></param>
    public void RegisterNetworkObject(NetworkObject pNetworkObject)
    {
        // Add the object to the list  -- TO THE LIST !!!
        networkObjects.Add(pNetworkObject);
        Debug.Log("--->>  GSM.RNO...Count of networkObjects: " + networkObjects.Count);   // 9/30/19
        //... Nov 1
        foreach (var client in clients)
        {
            SendObjectToSpawnTo(pNetworkObject, client);
        }
    }

    /// <summary>
    /// Send a message to the client to spawn an object into its scene
    /// </summary>
    ///<param name="pClient"></param>
    public void SendObjectToSpawnTo(NetworkObject pNetworkObject, IClient pClient)
    {
        // Spawn data to send
        SpawnMessageModel spawnMessageData = new SpawnMessageModel
        {
            networkID = pNetworkObject.id,
            resourceID = pNetworkObject.resourceId,
            x = pNetworkObject.gameObject.transform.position.x,
            //...x = 2.5f,
            y = pNetworkObject.gameObject.transform.position.y
            //...y = 0.75f
        };

        // Create the message
        using (Message m = Message.Create
                (NetworkTags.InGame.SPAWN_OBJECT,       // Tag
                spawnMessageData)                       // Data
              )
        {
            // Send the message in TCP mode (Reliable)
            pClient.SendMessage(m, SendMode.Reliable);
        }
    }

    /// <summary>
    /// Send a message with all objects to spawn
    /// </summary>
    /// <param name="pClient"></param>
    public void SendAllObjectsToSpawnTo(IClient pClient)
    {
        foreach (NetworkObject networkObject in networkObjects)
            SendObjectToSpawnTo(networkObject, pClient);
    }

    /// <summary>
    /// Instantiate instance of a resource
    /// </summary>
    /// <param name="resourceName"></param>
    /// <param name="position"></param>
    private void InstantiateResource(string resourcePath, Vector3 position, IClient client)
    {
        //Spawn the game object
         GameObject go = Resources.Load(resourcePath) as GameObject;
        // Initialize the client Id and start position on the associated NetworkObject - Nov 1
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.clientId = client.ID;
        no.startPosition = position;
        Instantiate(go, position, Quaternion.identity);
    }

    #endregion
}