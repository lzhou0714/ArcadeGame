// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class PlayerController : MonoBehaviour
// {
//     // Start is called before the first frame update
//     private float rotateAngle = 90f;
//     
//     public Rigidbody2D rb;
//     private Vector2 movement;
//     private float rotate;
//     public float fallspeed = 5;
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void FixedUpdate()
//     {
//         rb.velocity = movement*2;
//     }
//     void OnMove(InputValue moveValue)
//     {
//         movement = moveValue.Get<Vector2>();
//         if (movement.y == 1.0f)
//          movement = new Vector2(0, 0);
//
//
//     }
//
//     void OnButton1()
//     {
//         print("rotateLft"); 
//
//         transform.Rotate(0,0,-rotateAngle);
//     }
//
//     void OnButton2()
//     {
//         print("rotateRight"); 
//
//         transform.Rotate(0,0,rotateAngle);
//         
//     }
//     void OnButton3() { print("button3"); }
//     void OnButton4() { print("button4"); }
//     void OnButton5() { print("button5"); }
//     void OnButton6() { print("button6"); }
// }
