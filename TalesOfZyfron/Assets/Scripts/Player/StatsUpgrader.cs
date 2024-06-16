using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class StatsUpgrader : NetworkBehaviour
{
    GameManager gm;
    [SerializeField] NewPlayerController pc;
    [SerializeField] private FirstPersonCamera fpc;
    [SerializeField] private SwordSwingScript swordswing;
    [SerializeField] private Button maxHpButton;
    [SerializeField] private Button damageButton;


    [SerializeField] private SwordDamage swordDamage;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Ressources ressources;
    public GameObject sword;
    public GameObject Interface;
    public int heal_price = 25;
    public int maxhealth_price = 10;
    public int damage_price = 1;
    private bool IsActive = false;
    private void Start(){
        swordDamage = sword.GetComponent<SwordDamage>();
        playerHealth = GetComponent<PlayerHealth>();
        ressources = GetComponent<Ressources>();
        Interface.SetActive(false);
        UpdateButtonVisual(maxHpButton, "Max Hp +10\n-10 gold");
        UpdateButtonVisual(damageButton, "Damage +1\n-1 gold");
        gm =  GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        
    }
    
    /*private void Awake(){
        maxHpButton.onClick.AddListener(() => {

            if (ressources.goldCount < maxhealth_price) return;

            playerHealth.Max += 10;
            ressources.Pay(10);
        });

        damageButton.onClick.AddListener(() => {
            if (ressources.goldCount < damage_price) return;

            swordDamage.damage += 1;
            ressources.Pay(damage_price);
            damage_price = (int)(swordDamage.damage*1.25f);
            string text = $"Damage +1\n-{damage_price} gold";
            UpdateButtonVisual(damageButton, text);

        });

        healButton.onClick.AddListener(() => {
            if (ressources.goldCount < heal_price) return;

            playerHealth.Max += 10;
            ressources.Pay(heal_price);
            heal_price = playerHealth.Max/2;
            string text = $"Heal\n-{heal_price} gold";
            UpdateButtonVisual(healButton, text);
        });
    }*/

    public void Update(){
        if (Input.GetKeyDown(KeyCode.P) && !gm.gameStarted.Value) {
            ToggleInterface();
        }
    }
    public void AddMaxHp(){
        Debug.Log("addmaxhp called");
        if (ressources.goldCount < maxhealth_price) return;

        playerHealth.Max += 10;
        ressources.Pay(maxhealth_price);
        heal_price = playerHealth.Max / 4;
        string text = $"Heal\n-{heal_price} gold";
        playerHealth.Heal(10);
    }
    public void AddDamage(){
        Debug.Log("addDamage called");
        if (ressources.goldCount<damage_price) return;
            swordDamage.IncreaseDamage();
            ressources.Pay(damage_price);
            damage_price = (int)(swordDamage.damage*1.25f);
            string text = $"Damage +1\n-{damage_price} gold";
            UpdateButtonVisual(damageButton, text);
    }

    private void ToggleInterface(){
        IsActive = !IsActive;
        Interface.SetActive(IsActive);
        if (IsActive) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            fpc.enabled = false;
            pc.enabled = false;
            swordswing.enabled = false;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            fpc.enabled = true;
            pc.enabled = true;
            swordswing.enabled = true;
        }
    }

    private void UpdateButtonVisual(Button button, string text){
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;
    }
}