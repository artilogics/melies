using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Lamp : MonoBehaviour
{
    [Header("Led Emission")]
    public Color Emission_Off_ = new Color(0, 0, 0);
    public Color Emission_On = new Color(1, 1, 1);

    private Renderer rend;


    public bool B_AP_Emission_On(){
        if (rend == null)
            rend = GetComponent<Renderer>();
        //Debug.Log("Emission On");
        rend.material.SetColor("_EmissionColor", Emission_On);
        return true;
    }

    public bool B_AP_Emission_Off(){
        if (rend == null)
            rend = GetComponent<Renderer>();
        //Debug.Log("Emission Off");
        rend.material.SetColor("_EmissionColor", Emission_Off_);
        return true;
    }

    public bool B_AP_Emission_Change()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();


        if(rend.material.GetColor("_EmissionColor") ==  Emission_Off_)
            rend.material.SetColor("_EmissionColor", Emission_On);
        else
            rend.material.SetColor("_EmissionColor", Emission_Off_);

        //Debug.Log("Emission Change");
        return true;
    }

    public bool B_AP_SwitchALight(){
        GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
        return true;
    }

    public bool B_AP_Light_On()
    {
        GetComponent<Light>().enabled = true;
        return true;
    }

    public bool B_AP_Light_Off()
    {
        GetComponent<Light>().enabled = false;
        return true;
    }
}
