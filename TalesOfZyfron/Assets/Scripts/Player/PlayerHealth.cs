using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    
    public int Max {
        get => MaxHealth;
        set => MaxHealth = value;
    }
    public int Current {get => currHealth;}
    [SerializeField] private int MaxHealth;
    [SerializeField] TMP_Text Hpinfo;
    [SerializeField] private Image HpBarImage;
    private int currHealth;
    [SerializeField] private GameObject player;
    [SerializeField] private NewPlayerController pc;
    [SerializeField] private FirstPersonCamera fpc;
    [SerializeField] private GameObject deathScreen;
    private KeyCode debugKey = KeyCode.P;
    private KeyCode spawnKey = KeyCode.O;

    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        currHealth = MaxHealth;
        Hpinfo.text = "Hp:" + currHealth;
        isDead = false;
        HpBarImage.fillAmount = 1;
    }
    
    
    void Update()
    {
        if (!IsOwner) return;

        if (currHealth <= 0 && !isDead)
        {
            KillPlayer();
        }

        // Update the health display text
        Hpinfo.text = "Hp: " + currHealth;
        HpBarImage.fillAmount = (float)currHealth / MaxHealth;
    }

    public void TakeDamage(int  damage)
    {
        currHealth -=  damage;
        Debug.Log("Player took " + damage + " damage");
        Hpinfo.text = "Hp:" + currHealth;
        HpBarImage.fillAmount = (float)currHealth / MaxHealth;
    }
    public void Heal(int heal)
    {
        currHealth = MaxHealth;
        Debug.Log("Player healed" + heal + "hp");
    }
    public void IncreaseHP(){
        MaxHealth += 10;
        Heal(10);
    }
    private void KillPlayer()
    {
        if(!isDead)
        {
            isDead = true;
            pc.currState = NewPlayerController.MovementState.dead;
            pc.enabled = false;
            fpc.enabled = false;
            deathScreen.SetActive(true);
            GameObject gameManager = GameObject.FindGameObjectWithTag("Game Manager");
            if (gameManager != null)
            {
                GameManager gm = gameManager.GetComponent<GameManager>();
                if (gm != null)
                {
                    gm.UpdateAlvivePlayersServerRpc(-1);
                }
            }
        }
       
   
    }
    public void SpawnPlayer()
    {
        if (isDead)
        {
            isDead = false;
            currHealth = MaxHealth;
            pc.enabled = true;
            fpc.enabled = true;
            deathScreen.SetActive(false);
            pc.SetLoadingScreen(true);
            Hpinfo.text = "Hp:" + currHealth;


            GameObject gameManager = GameObject.FindGameObjectWithTag("Game Manager");
            if (gameManager != null)
            {
                GameManager gm = gameManager.GetComponent<GameManager>();
                if (gm != null)
                {
                    gm.UpdateAlvivePlayersServerRpc(1);
                }
            }
        }

    }
    
}
