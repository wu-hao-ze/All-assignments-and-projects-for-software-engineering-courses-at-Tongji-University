using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCube : MonoBehaviour {
    [HideInInspector]
    public GameObject turretGo;//保存当前cube身上的炮台
    [HideInInspector]
    public TurretData turretData;
    [HideInInspector]
    public bool isUpgraded = false;

    public GameObject buildEffect;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void BuildTurret(TurretData turretData)
    {
        this.turretData = turretData;
        isUpgraded = false;
        turretGo = GameObject.Instantiate(turretData.turretPrefab, transform.position, Quaternion.identity);
        GameObject effect = GameObject.Instantiate(buildEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
    }

    public void UpgradeTurret()
    {
        if(isUpgraded==true)return;

        Destroy(turretGo);
        isUpgraded = true;
        turretGo = GameObject.Instantiate(turretData.turretUpgradedPrefab, transform.position, Quaternion.identity);
        GameObject effect = GameObject.Instantiate(buildEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
    }
    
    public void DestroyTurret()
    {
        Destroy(turretGo);
        isUpgraded = false;
        turretGo = null;
        turretData=null;
        GameObject effect = GameObject.Instantiate(buildEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
    }

    void OnMouseEnter()
    {

        if (turretGo == null && EventSystem.current.IsPointerOverGameObject()==false)
        {
            renderer.material.color = Color.red;
        }
    }
    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }
}
