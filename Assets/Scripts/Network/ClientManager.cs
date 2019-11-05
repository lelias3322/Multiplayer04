using DarkRift.Client;      //... needed for MessageReceivedEventArgs...
using DarkRift.Client.Unity;
using UnityEngine;
using System.Net;
using UnityEngine.SceneManagement;
using Utilities;

public class ClientManager : MonoBehaviourSingletonPersistent<ClientManager>
{

    #region Properties

    /// <summary>
    /// Reference to the DarkRift2 client
    /// </summary>
    public UnityClient clientReference;
    
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        base.Awake();

        /////////////////////////
        /// Properties initialization
        clientReference = GetComponent<UnityClient>();

        Screen.SetResolution(1000, 700, false);          //...trying to get rid of full screen 8/20/19 at 9:00 pm  orig 500, 350
    }

     // Start is called before the first frame update
    void Start()
    {
        /////////////////////////
        /// Load the game scene
        SceneManager.LoadScene("MainGameScene", LoadSceneMode.Additive);

        ////////////////////////
        /// Suscribe to events - 10/1 - Part 10
        /// We've added a listener on message received called SpawnGameObjects
        clientReference.MessageReceived += SpawnGameObjects;

        ///////////////////////
        /// Connect to the server manually - 10/1 - Part 10
        clientReference.ConnectInBackground         //...CONNECT IN BACKGROUND !! 
        (
            IPAddress.Parse("192.168.1.104"),      //127.0.0.1         // at Tom's 10.0.0.200
            4296,
            DarkRift.IPVersion.IPv4,
            null
        );
    }

    ///<summary>
    /// Spawn object if message received is tagged as SPAWN_OBJECT
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SpawnGameObjects(object sender, MessageReceivedEventArgs e)
    {
        if (e.Tag == NetworkTags.InGame.SPAWN_OBJECT)
        {
            // Get message data
            SpawnMessageModel spawnMessage =
                e.GetMessage().Deserialize<SpawnMessageModel>();
            
            // Spawn the game object
            string resourcePath = 
                NetworkObjectDictionary.GetResourcePathFor(spawnMessage.resourceID);
            
            GameObject go = Resources.Load(resourcePath) as GameObject;
            
            go.GetComponent<NetworkObject>().id = spawnMessage.networkID;

            Debug.Log("--->> from CM: x position " + spawnMessage.x);
            Debug.Log("--->> from CM: y position " + spawnMessage.y);
           
            Instantiate(go, new Vector3(spawnMessage.x, spawnMessage.y, 0),
                Quaternion.identity);
        }
    }
    #endregion
}
