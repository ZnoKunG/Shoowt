using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    public GameObject bullet;

    public float shootForce, upwardForce;

    // Gun Properties
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public bool allowInvoke = true;

    [SerializeField] Camera playerCam;
    [SerializeField] Transform attackPoint;

    // Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    void Awake() {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    void Update() {
        GetInput();
        if (ammunitionDisplay != null) {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    void GetInput() {
        if (allowButtonHold){
            shooting = Input.GetKey(KeyCode.Mouse0);
        } else {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) {
            bulletsShot = 0;
            Shoot();
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        // Automatically reload
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();
    }

    void Shoot() {
        readyToShoot = false;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Middle of the screen
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit)) {
            targetPoint = hit.point;
        } else {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // Calculate Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate Direction with Spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate bullet / projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        currentBullet.transform.forward = directionWithSpread.normalized;

        Destroy(currentBullet, 5f);

        // Add Forces to Bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardForce, ForceMode.Impulse);

        // Instantiate muzzle flash (if have one)
        if (muzzleFlash != null) {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }
        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke) {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        // For more than one bullet per tap -> repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0) {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    void ResetShot() {
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reload() {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished() {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
