﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Parameters")]
    [SerializeField] private float _life = 0;
    [SerializeField] private float _maxLife = 50;
    [SerializeField] private float _rotationSpeed = 0;
    [SerializeField] private float _shootRate = 0;
    [SerializeField] private float _damage = 0;
    [SerializeField] private bool _canShoot = true;
    [Space]
    [Header("Enemy Checker")]
    [SerializeField] protected LayerMask _enemyLayer;
    [SerializeField] protected float _sphereRadius;
    [SerializeField] private Transform _target;
    public GameObject rotor;
    private Collider[] _enemyCollider;
    

    [SerializeField] private Transform _muzzle;
    private float _shootCooldown = 0;
    private Enemy _enemyTest;
    private bool picked = false;

    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    protected GameObject mainTurret;

    //Return public turret life
    public float GetTurretLife
    {
        get { return _life; }
    }

    public bool isPicked()
    {
        return picked;
    }

    public void SetPicked(bool p)
    {
        picked = p;
    }

    public void ResetTarget()
    {
        _target = null;
    }


    void Start()
    {
        mainTurret = GameObject.Find("MainTurret");
    }

    // Update is called once per frame
    void Update()
    {
        if(mainTurret != null)
        {
        CheckEnemyInRange();
        FollowEnemy();
        Attack();
        Die();
        }
    }

    /// <summary>
    /// Check if enemy is inside a sphere vision range
    /// </summary>
    protected void CheckEnemyInRange()
    {
       
        if(_target == null)
        {
            _enemyCollider = Physics.OverlapSphere(transform.position, _sphereRadius, _enemyLayer);
            
            foreach(Collider c in _enemyCollider)
            {
                if(c.gameObject.activeInHierarchy)
                {
                    _target = c.transform;
                    break;
                }
                
            }
        }else
        {
            if(!_target.gameObject.activeInHierarchy)
            {
                _enemyCollider = Physics.OverlapSphere(transform.position, _sphereRadius, _enemyLayer);
                _target = null;
                _enemyTest = null;
                foreach(Collider c in _enemyCollider)
                {
                    if(c.gameObject.activeInHierarchy && Vector3.Distance(transform.position, c.transform.position) < _sphereRadius)
                    {
                        _target = c.transform;
                        break;
                    }
                    
                }
            }
        }
        
        
        //Debug.Log(enemyIndex);
        //if(_enemyCollider.Length > 0)
            
    }

    /// <summary>
    /// Rotate the turret smoothly to enemy direction
    /// </summary>
    protected void FollowEnemy()
    {
        if(_target!= null)
        {
            if (_target.gameObject.activeInHierarchy)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_target.position.x - rotor.transform.position.x, 0, _target.position.z - rotor.transform.position.z), Vector3.up);
                rotor.transform.rotation = Quaternion.Slerp(rotor.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            if (_enemyTest == null && _target.gameObject.activeInHierarchy)
                _enemyTest = _target.GetComponent<Enemy>();
        }
    }

    /// <summary>
    /// Attack to an enemy by raycasting hit
    /// </summary>
    protected void Attack()
    {    
        if (!_canShoot)
        {
            _shootCooldown -= Time.deltaTime;
            if(_shootCooldown <= 0)
            {
                Debug.Log("OnFire");
                _canShoot = true;
                _shootCooldown = _shootRate;
            }
        }

        RaycastHit hit;
        if(Physics.Raycast(_muzzle.position, _muzzle.transform.forward,out hit, _sphereRadius, _enemyLayer))
        {
            //Debug.DrawRay(_muzzle.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 1);
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            //Debug.Log("Hiteo");
            if (_canShoot)
            {
                Debug.DrawRay(_muzzle.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 1);
                
                enemy.AplicarDaño(_damage);

                if(enemy.GetTarget() != gameObject)
                {
                    enemy.SetTarget(gameObject);
                }
                _particleSystem.Play(true);
                _audioSource.Play();
                _canShoot = false;
               //Debug.Log("SHOOT!");
            }
           /* if (_canShoot && _enemyTest != null)
            {
                _canShoot = false;
                _enemyTest.AplicarDaño(5);

                if(_enemyTest.GetTarget() != gameObject)
                {
                    _enemyTest.SetTarget(gameObject);
                }
               //Debug.Log("SHOOT!");
            }*/

            Debug.DrawRay(_muzzle.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
        }
    }

    /// <summary>
    /// Destroy the turret
    /// </summary>
    protected void Die()
    {
        if (_life <= 0)
        {
            //Debug.Log("Torreta destruida");
            Destroy(gameObject);
        }
    }

    public void ApplyDamage(float dmg)
    {
        _life -= dmg;
        if(_life > _maxLife)
        {
            _life = _maxLife;
        }
    }
}
