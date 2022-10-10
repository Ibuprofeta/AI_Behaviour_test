using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    [SerializeField] private Slider healthBar;

    public void SetHealth(int amount){
        currentHealth = maxHealth = amount;
    }

    public IEnumerator TakeDamageDelayed(int amount, float delay){
        yield return new WaitForSeconds(delay);
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        if (currentHealth == 0){
            SetHealth(20);
        }
        OnHealthChange();
    }

    public void OnHealthChange(){
        healthBar.value = (float) currentHealth / maxHealth;
    }

    public void Heal(int amount){
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChange();
    }

}
