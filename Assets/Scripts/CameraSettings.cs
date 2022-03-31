using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    Driver driver;
    Move move;
    Transform selectedCar;

    private void Update()
    {
        RaycastHit[] hits = new RaycastHit[5];
        int carLayer = LayerMask.GetMask("Car");
        Debug.Log(carLayer);
        int layerMask = (1 << carLayer);
        for (int i = 0; i < 5; i++)
        {
            var ray = selectedCar.GetChild(2 + i);
            var line = ray.GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, ray.position);
            Vector3 forward = ray.TransformDirection(Vector3.forward) * 15;
            if (Physics.Raycast(new Ray(ray.position, forward), out RaycastHit raycastHit, 15f, layerMask))
            {
                Debug.Log(raycastHit.transform.name);
                line.SetPosition(1, raycastHit.point);
                hits[i] = raycastHit;
            }
            else
            {
                Vector3 secondPoint = new Vector3(ray.position.x + forward.x, ray.position.y, ray.position.z + forward.z);
                line.SetPosition(1, secondPoint);
                hits[i] = raycastHit;
            }
        }
        driver.SetRayHit(hits);
        driver.SetSpeed(move.motor);
    }
    void FixedUpdate()
    {
        selectedCar = CarsController.cars[0];
        driver = selectedCar.GetComponent<Driver>();
        move = selectedCar.GetComponent<Move>();
        transform.LookAt(CarsController.cars[0]);
        transform.position = new Vector3(CarsController.cars[0].position.x,
            CarsController.cars[0].position.y + 15, CarsController.cars[0].position.z + 20);
    }

}
