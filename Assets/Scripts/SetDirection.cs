using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirection : MonoBehaviour
{
    public CreateLevels.direction direction;
    void OnTriggerEnter2D(Collider2D other)
    {
        CreateLevels.instance.currentDirection = direction;
            
    }
}
