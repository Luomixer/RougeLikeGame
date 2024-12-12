using UnityEngine;

public class HitScanWeapon : MonoBehaviour
{
    public float weaponRange = 100f; // Maximum range of the weapon
    public int damage = 20; // Damage dealt by the weapon
    public Camera playerCamera; // Reference to the player's camera

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Check if the fire button is pressed
        {
            FireWeapon();
        }
    }

    void FireWeapon()
    {
        RaycastHit hit;
        // Create a ray from the camera's position in the direction it's facing
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, weaponRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Check if the object hit has a health component
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Apply damage
            }
        }
    }
}

// Health class for demonstration purposes
public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        gameObject.SetActive(false);
    }
}