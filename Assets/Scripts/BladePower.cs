using System.Collections;
using UnityEngine;

public class BladePower : Weapon
{
    private GameObject bladePrefab;  
    private int numberOfBlades = 2;  
    private float radius = 1.5f;  
    private float rotationSpeed = 200f;  
    private GameObject[] blades;  
    private float angleStep;  

    private class Blade : MonoBehaviour
    {
        private int damage;

        public void SetDamage(int dmg) 
        {
            damage = dmg;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null && other.GetComponent<Enemy>() != null) 
            {
                Enemy otherEnemy = other.GetComponent<Enemy>();
                otherEnemy.TakeDamage(damage);
                SoundManager.Instance.PlaySound("blade_sound");
            }
        }
    }

    private void Start()
    {
        type = WeaponType.BLADE;
        isMaxLevel = false;
        maxLevel = 5;
        damage = 5;
        bladePrefab = Resources.Load<GameObject>("Prefabs/blade"); 
        bladePrefab.AddComponent<Blade>();

        InitializeBlades();

        Activate();
    }

    void Update()
    {
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }
    }

    public override void Activate()
    {
        isActive = true;
        StartCoroutine(RotateBlades());
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines(); 
    }

    private IEnumerator RotateBlades()
    {
        while (isActive)  
        {
            for (int i = 0; i < numberOfBlades; i++)
            {

                // position using radius
                Vector3 newPos = new Vector3(
                    Mathf.Cos(Mathf.Deg2Rad * (i * angleStep + Time.time * rotationSpeed)) * radius,
                    Mathf.Sin(Mathf.Deg2Rad * (i * angleStep + Time.time * rotationSpeed)) * radius,
                    0f
                );

                blades[i].transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

                // keep the blade at the correct radius from the player
                blades[i].transform.position = transform.position + newPos;
            }

            yield return null;
        }
    }

    public override void LevelUp()
    {
        currLevel++;
        numberOfBlades++;
        InitializeBlades();
    }

    private void InitializeBlades()
    {
        if (bladePrefab == null)
        {
            Debug.LogError("bladePrefab is null in InitializeBlades");
            bladePrefab = Resources.Load<GameObject>("Prefabs/blade"); 
            bladePrefab.AddComponent<Blade>();
        }

        // Destroy old blades before creating new ones
        if (blades != null)
        {
            foreach (GameObject oldBlade in blades)
            {
                if (oldBlade != null) Destroy(oldBlade);
            }
        }

        // create and place the blades in their starting positions
        angleStep = 360f / numberOfBlades;  
        blades = new GameObject[numberOfBlades]; 

        for (int i = 0; i < numberOfBlades; i++)
        {
            float angle = i * angleStep;  
            blades[i] = Instantiate(bladePrefab, transform.position, Quaternion.Euler(0f, 0f, angle));
            blades[i].transform.SetParent(transform);  

            // set damage for blade
            Blade bladeScript = blades[i].GetComponent<Blade>();
            if (bladeScript != null)
            {
                bladeScript.SetDamage(damage);
            }
        }
    }

    public override void DoDamage()
    {
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }
}
