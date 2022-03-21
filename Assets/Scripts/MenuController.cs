using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class MenuController : MonoBehaviour
{
    public void LoadModelling()
    {
        SceneManager.LoadScene("SelectMap");
    }

    public void ExitApplication()
    {
        /*XmlSerializer formatter = new XmlSerializer(typeof(List<Cell[][]>));
        using(FileStream fs = new FileStream("Maps.xml", FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, ModelMap.maps);
        }
        Application.Quit(1);*/
    }
}
