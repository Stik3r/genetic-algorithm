using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSettings : MonoBehaviour
{
    GameObject selectedCar;
    public GameObject score;


    private void Start()
    {
        selectedCar = CarsController.cars[0];
    }
    private void Update()
    {
        if (selectedCar != null)
        {
            RaycastHit[] hits = new RaycastHit[5];
            int carLayer = LayerMask.GetMask("Car", "Trigger");
            int layerMask = (1 << carLayer);
            for (int i = 0; i < 5; i++)
            {
                var ray = selectedCar.transform.GetChild(2 + i);
                var line = ray.GetComponent<LineRenderer>();
                line.positionCount = 2;
                line.SetPosition(0, ray.position);
                Vector3 forward = ray.TransformDirection(Vector3.forward) * 15;
                if (Physics.Raycast(new Ray(ray.position, forward), out RaycastHit raycastHit, 15f, layerMask))
                {
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
        }
    }
    void FixedUpdate()
    {
        if(selectedCar == null)
        {
            selectedCar = CarsController.cars[0];
        }
        if(!selectedCar.GetComponent<Driver>().Alive)
            foreach(var car in CarsController.cars)
            {
                if (car.GetComponent<Driver>().Alive)
                {
                    foreach (Transform child in selectedCar.transform)
                    {
                        if (child.name.Contains("ray"))
                        {
                            child.GetComponent<LineRenderer>().enabled = false;
                        }
                    }
                    selectedCar = car;
                    break;
                }
            }
        transform.LookAt(selectedCar.transform);
        transform.position = new Vector3(selectedCar.transform.position.x,
            selectedCar.transform.position.y + 15,
            selectedCar.transform.position.z + 20);
        score.GetComponent<Text>().text = selectedCar.GetComponent<Driver>().score.ToString();
    }

}
