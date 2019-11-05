using DarkRift;
using DarkRift.Server;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class NetworkBouncyBall : NetworkObject
{
    #region Properties
    public Rigidbody rigidbodyReference;
    //...public int kount = 0;       //...to limit Debug.Log output


 
    ///<summary>
    /// Tick counted by the client
    /// </summary>
    public int clientTick = -1;

 
    #endregion


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        //////////////////////////////
        // Get references
        rigidbodyReference = GetComponent<Rigidbody>();

        // If we are on client side
        if (!Equals(ClientManager.instance, null))
        {
            ////////////////////
            /// Suscribe to events
            ClientManager.instance.clientReference.MessageReceived +=
                            UpdateFromServerState;
         }
    }

    private void OnDestroy()
    {
        // If we are on client side
        if (!Equals(ClientManager.instance, null))
        {
            ClientManager.instance.clientReference.MessageReceived -=
                            UpdateFromServerState;
        }
    }


    private void FixedUpdate()
    {
 
        // If we are on server side
        if (!Equals(GameServerManager.instance, null))
        {
 
            if (GameServerManager.instance.currentTick % 10 == 0)
                SendBallPositionToClients();
        }
        
        //...else if (!Equals(ClientManager.instance, null))
        else if (!Equals(ClientManager.instance, null) && clientTick != -1)
        {
            clientTick++;
       
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    ///<summary>
    /// Send ball server position to all clients
    /// </summary>
    private void SendBallPositionToClients()
    {
        //Create the message
        BouncyBallSyncMessageModel bouncyBallPositionMessaageData = new
                BouncyBallSyncMessageModel(
                base.id,
                GameServerManager.instance.currentTick,
                rigidbodyReference.transform.position,
                rigidbodyReference.velocity);

        //...Time Stamps per Tom McDonnell Oct 27
        bouncyBallPositionMessaageData.timeSent = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;

        // create the message
        using (Message m = Message.Create(
                NetworkTags.InGame.BOUNCY_BALL_SYNC_POS,        // Tag
                bouncyBallPositionMessaageData)                 // Data
        )
 
        {
            foreach (IClient client in
                GameServerManager.instance.serverReference.Server.ClientManager.GetAllClients())
            {
                client.SendMessage(m, SendMode.Reliable);
            }
        }
    }

    ///<summary>
    /// update from the sever state
    /// </summary>
    ///<param name="sender"<>/param>
    ///<param name="e"></param>
    private void UpdateFromServerState(object sender,
                    DarkRift.Client.MessageReceivedEventArgs e)
    {
        if (e.Tag == NetworkTags.InGame.BOUNCY_BALL_SYNC_POS)
        {
            // Get message data
            BouncyBallSyncMessageModel syncMessage =
                    e.GetMessage().Deserialize<BouncyBallSyncMessageModel>();
            // Time Stamp per Tom McDonnell Oct 27
            syncMessage.timeReceived = new TimeSpan(DateTime.Now.Ticks).TotalMilliseconds;

            if (id == syncMessage.networkID)
            {
                // If the distance between client and server ball position is beyone nominal constraint
                if (Vector3.Distance(rigidbodyReference.transform.position, syncMessage.position) >= 0.05f)
                {
                  // Update data
                    rigidbodyReference.velocity = syncMessage.velocity;
                    rigidbodyReference.transform.position = syncMessage.position;
                    clientTick = syncMessage.serverTick;
                    //...lastReceivedMessage = syncMessage;
                 }
            }
         }
      }
 }
