using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMashine : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(CarsController.cars[0]);
        transform.position = new Vector3(CarsController.cars[0].position.x,
            CarsController.cars[0].position.y + 15, CarsController.cars[0].position.z + 20);
    }
}
