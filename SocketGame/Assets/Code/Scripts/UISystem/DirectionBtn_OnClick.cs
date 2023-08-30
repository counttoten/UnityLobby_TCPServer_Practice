using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    RIGHT,
    LEFT
}


public class DirectionBtn_OnClick : MonoBehaviour
{
    public Direction Direction;
    LobbyManager lm;

    private void Start()
    {
        lm = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    public void OnDirectionClick()
    {
        lm.RotateCamera(Direction == Direction.LEFT);
    }
}
