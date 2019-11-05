using DarkRift;
using UnityEngine;

public class BouncyBallSyncMessageModel : IDarkRiftSerializable
{

    #region Properties
    public int networkID;           //...Id of the object in the server
    public int serverTick;
    public Vector3 position;
    public Vector3 velocity;
    private int p1;
    private int p2;
    private Vector3 vector31;
    private Vector3 vector32;

    public double timeSent;
    public double timeReceived;
    public double timeProcessed;

    //...Is this constructor necessary for NetworkBouncyBall? - line 46
    public BouncyBallSyncMessageModel(int p1, int p2, Vector3 vector31, Vector3 vector32)
    {
        // TODO: Complete member initialization  - should not have ignored this 10/29
        this.networkID = p1;
        this.serverTick = p2;
        this.position = vector31;
        this.velocity = vector32;
    }

    //...another constructor needed
    public BouncyBallSyncMessageModel()
    {

    }

    #endregion

    #region IDarkRiftSerializable implementation
    public void Deserialize(DeserializeEvent e)
    {
        networkID = e.Reader.ReadInt32();
        serverTick = e.Reader.ReadInt32();
        position = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(),
                                e.Reader.ReadSingle());
        velocity = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(),
                                e.Reader.ReadSingle());
        timeSent = e.Reader.ReadDouble();
        timeReceived = e.Reader.ReadDouble();
        timeProcessed = e.Reader.ReadDouble();
    }

    public void Serialize(SerializeEvent e)
    {
        // Write id of the network object
        e.Writer.Write(networkID);

        // Write the serverTick of the network object
        e.Writer.Write(serverTick);

        // Write position
        e.Writer.Write(position.x);
        e.Writer.Write(position.y);
        e.Writer.Write(position.z);

        e.Writer.Write(velocity.x);
        e.Writer.Write(velocity.y);
        e.Writer.Write(velocity.z);

        // Write Time Stamps
        e.Writer.Write(timeSent);
        e.Writer.Write(timeReceived);
        e.Writer.Write(timeProcessed);
    }
    #endregion
}
