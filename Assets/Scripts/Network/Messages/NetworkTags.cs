//...using System.Collections;
//...using System.Collections.Generic;
//...using UnityEngine;

/// <summary>
///  Classes that stores all tags       This was started in Part 8  - added to Oct 11, 2019
/// </summary>

public static class NetworkTags
{
    /// <summary>
    /// Tags used in game
    /// </summary>
    public struct InGame
    {
        /////////////////////////////////
        // Common
        public const ushort SPAWN_OBJECT = 1001;

        ////////////////////////////////
        // Bouncy Ball
        public const ushort BOUNCY_BALL_SYNC_POS = 2001;
    }
}
