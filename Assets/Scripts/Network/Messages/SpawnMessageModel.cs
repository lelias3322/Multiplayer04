using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;

public class SpawnMessageModel : IDarkRiftSerializable
{

    #region Properties
    /// <summary>
    /// Id of the object in the server
    /// </summary>
    public int networkID { get; set; }

    /// <summary>
    /// X position
    /// </summary>
    public float x { get; set; }

    /// <summary>
    /// Y position
    /// </summary>
    public float y { get; set; }

    /// <summary>
    /// Resource to spawn 10/1 - part 10
    /// </summary>
    public int resourceID { get; set; }
    #endregion

    #region DarkRift IDarkRiftSerializable implemention
    public void Deserialize(DeserializeEvent e)
    {
        networkID = e.Reader.ReadInt32();
        resourceID = e.Reader.ReadInt32();  // 10/1 - Part 10
        x = e.Reader.ReadSingle();
        y = e.Reader.ReadSingle();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(networkID);
        e.Writer.Write(resourceID);  // 10/1 - Part 10
        e.Writer.Write(x);
        e.Writer.Write(y);
    }
    #endregion
}
