﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameObject jugador;
    private NavMeshAgent enemigo;
    private GameObject target;
    private float distanceTarget;

    [SerializeField]
    private float daño;
    [SerializeField]
    private float vida;
    public GameObject trashPref;
    [SerializeField]
    private EnemiesCount countEnemies;

    private float temporizadorDeDaño = 1.0f;
    private bool estasDentro;

    private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        countEnemies = GameObject.Find("EnemiesCount").GetComponent<EnemiesCount>();
        jugador = GameObject.Find("MainTurret");
        enemigo = GetComponent<NavMeshAgent>();
        target = jugador;
        countEnemies = GameObject.Find("EnemiesCount").GetComponent<EnemiesCount>();
        _audioSource = GameObject.Find("EnemyExplosion").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(jugador != null)
    	{
    	if(target == null)
    	{
    		target = jugador;
    	}else if(target != jugador)
    	{
    		if(target.GetComponent<Turret>().isPicked() == true)
    		{
    			target = jugador;
    			estasDentro = false;
    		}
    	}

    	distanceTarget = Vector3.Distance(transform.position, target.transform.position);

    	if(distanceTarget <= 1f)
    	{
    		estasDentro = true;
    		
    	}
    	else
    	{
    		estasDentro = false;
    	}


        if(!estasDentro)
            enemigo.destination = target.transform.position;


        else
        {
            enemigo.destination = this.transform.position;
            if(temporizadorDeDaño <= 0)
            {
            	if(target == jugador)
            	{
                	jugador.GetComponent<MainTurret>().ApplyDamage(daño);
            	}else
            	{
            		target.GetComponent<Turret>().ApplyDamage(daño);
            	}

                temporizadorDeDaño = 1.0f;
            }
            temporizadorDeDaño = temporizadorDeDaño - Time.deltaTime;
        }
        Die();

        if (_particleSystem != null)
        {
            if (_particleSystem.isStopped)
            {
                _particleSystem.gameObject.SetActive(false);
            }
        }
    	}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8 || other.tag == "Torret")
        {
        	if(other.gameObject == target)
        	{
        		//estasDentro = true;
        	}
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 || other.tag == "Torret")
        {
            if(other.gameObject == target)
        	{
        		//estasDentro = false;
        	}
        }
    }

    public void AplicarDaño(float dmg)
    {
        vida -= dmg;
        //Debug.Log(vida);
    }

    public float getLife()
    {
        return vida;
    }

    public void SetTarget(GameObject t)
    {
    	target = t;
    }

    public GameObject GetTarget()
    {
    	return target;
    }

    private void Die()
    {
        if (vida <= 0)
        {
            countEnemies.decrementEnemiesCount();
        	Instantiate(trashPref, transform.position, Quaternion.identity);
        	target = null;
        	vida = 20;
            _audioSource.Play();
            GameObject particle = ObjectPooler.SharedInstance.GetPooledObject(Manager.particleTag);
            if (particle != null)
            {
                _particleSystem = particle.GetComponent<ParticleSystem>();
                particle.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
                particle.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}
