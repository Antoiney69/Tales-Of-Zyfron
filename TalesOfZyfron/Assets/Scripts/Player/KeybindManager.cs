using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KeybindManager : NetworkBehaviour
{
   
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode escapeKey = KeyCode.Escape;
    public KeyCode debugKey = KeyCode.P;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftClick = KeyCode.Mouse0;
    public KeyCode respawn = KeyCode.G;

}
