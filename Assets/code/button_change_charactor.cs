using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class button_change_charactor : MonoBehaviour
{

    public GameObject charactor_hutao;
    public GameObject charactor_robot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   //ßx“ñ™CÆ÷ÈË
    public void Change_Character_button1(GameObject _settingPanel)
    {
        _settingPanel.SetActive(false);
        charactor_robot.transform.position = new Vector3(0, 0, 0); 
        charactor_hutao.transform.position = new Vector3(100, 0, 0); 
    }
    //ßx“ñ„ÓÎï
    public void Change_Character_button2(GameObject _settingPanel)
    {
        _settingPanel.SetActive(false);
        charactor_hutao.transform.position = new Vector3(0, 0, 0);
        charactor_robot.transform.position = new Vector3(100, 0, 0);
    }
}
