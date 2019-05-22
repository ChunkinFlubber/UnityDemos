﻿using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField]
    float LifeTime = 10.0f;
    float CurrLifeTime;
	[SerializeField]
	int Damage = 10;
	[SerializeField]
	float CritMultiplier = 2.0f;
	float CritPercent = 0.0f;

	float Speed;
    Vector3 Direction;
    Collider Body;
    TrailRenderer Trail;
	Weapon MyWeapon = null;

	public delegate void ProjectileEvent(GameObject obj);
	public delegate void ProjectileReturnEvent(Projectile obj);

	public ProjectileEvent ProjectileHit = delegate { };
	public ProjectileReturnEvent ProjectileReturn = delegate { };

	void Awake()
    {
        Body = GetComponent<Collider>();
		Body.enabled = false;
        Trail = GetComponentInChildren<TrailRenderer>();
    }

    void Update()
    {
        transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
        CurrLifeTime += Time.deltaTime;
        if(CurrLifeTime > LifeTime)
        {
			ProjectileReturn(this);
        }
    }

    public void Setup(Weapon owner, Vector3 direction, float speed = 10.0f, float critPercent = 0.0f)
    {
		MyWeapon = owner;
        Direction = direction;
        Speed = speed;
		CritPercent = critPercent;
        CurrLifeTime = 0;
        Trail.Clear();
		Body.enabled = true;
		gameObject.SetActive(true);
    }

	private void OnTriggerEnter(Collider collider)
	{
		HealthSystem HS = collider.gameObject.GetComponent<HealthSystem>();
		if(collider.gameObject != MyWeapon.Owner)
		{
			if(HS)
			{
				bool crit = Random.Range(0.01f,1) <= CritPercent;
				HS.ModifyHealth(Damage, MyWeapon.DamageTypes, transform.position, crit);
			}
			Debug.Log(collider.gameObject);
			ProjectileHit(collider.gameObject);
			ProjectileReturn(this);
		}
	}
}
