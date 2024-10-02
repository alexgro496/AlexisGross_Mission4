using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    public AudioSource SlingShotSound;

    private LineRenderer lineRenderer;

    [SerializeField] private LineRenderer RubberBand;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    // void Start(){
    //     RubberBand.SetPosition(0, firstPoint.position);
    //     RubberBand.SetPosition(2, secondPoint.position);
    // }

    void Awake(){
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    
    void OnMouseEnter(){
        // print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit(){
        // print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown(){
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update(){
        if(!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z -= Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        // Vector3 projPos = mouseDelta;
        // Vector3 additionPos = new Vector3(7, 5, 0);

        projectile.transform.position = projPos;

        RubberBand.enabled = true;
        RubberBand.SetPosition(0, (new Vector3 (firstPoint.position.x, firstPoint.position.y + 1)));
        RubberBand.SetPosition(2, (new Vector3 (secondPoint.position.x, secondPoint.position.y + 1)));
        RubberBand.SetPosition(1, projectile.transform.position);

        if(Input.GetMouseButtonUp(0)){
            aimingMode = false;
            RubberBand.enabled = false;
            SlingShotSound.Play();

            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile;

            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }
}
