using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private CharacterController _controller;
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _gravity = 9.81f;
    [SerializeField]
    private GameObject _muzzleFlash;
    [SerializeField]
    private GameObject _hitMarkerPrefab;
    [SerializeField]
    private AudioSource _weaponSound;
    [SerializeField]
    private int _currentAmmo;
    private int maxAmmo = 150;
    [SerializeField]
    private bool _isReloading = false;

    public bool hasCoin = false;

    private UIManager _uIManager;
    [SerializeField]
    private GameObject _weapon;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _currentAmmo = maxAmmo;
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetMouseButton(0) && _currentAmmo > 0 && _isReloading == false)
        {
            Shoot();
        }
        else
        {
            _muzzleFlash.SetActive(false);
            _weaponSound.Stop();
        }

        if (Input.GetKeyDown(KeyCode.R) && _isReloading == false)
        {
            _isReloading = true;
            StartCoroutine(Reload());
        }
        LockedMouse();
        CalculateMovement();
    }

    void Shoot()
    {
        _muzzleFlash.SetActive(true);
        _currentAmmo--;
        _uIManager.UpdateAmmo(_currentAmmo);
        if (_weaponSound.isPlaying == false)
        {
            _weaponSound.Play();
        }
        Ray rayOrgin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrgin, out hitInfo))
        {
            Debug.Log("Hit: " + hitInfo.transform.name);
            GameObject hitMarker = Instantiate(_hitMarkerPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(hitMarker, 1f);
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        Vector3 velocity = direction * _speed;
        velocity.y -= _gravity;
        velocity = transform.transform.TransformDirection(velocity);
        _controller.Move(velocity* Time.deltaTime);
    }

    void LockedMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(1.5f);
        _currentAmmo = maxAmmo;
        _uIManager.UpdateAmmo(_currentAmmo);
        _isReloading = false;
    }

    public void EnableWeapons()
    {
        _weapon.SetActive(true);
    }


}
