using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Sword;
    private Animator swordAnimator;
    private KeybindManager keybind;
    private NewPlayerController playerController;
    private bool canAttack = true;
    public float attackCooldown = 1.0f; // Cooldown duration in seconds
    public AudioSource audioSource;
    public List<AudioClip> swing;

    // Start is called before the first frame update
    void Start()
    {
        swordAnimator = Sword.GetComponent<Animator>();
        keybind = GetComponent<KeybindManager>();
        playerController = GetComponent<NewPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
        {
            StartCoroutine(SwordSwing());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Sword.GetComponent<Animator>().Play("running");    
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Sword.GetComponent<Animator>().Play("running");    
            }
            else
            {
                Sword.GetComponent<Animator>().Play("idle");
            }
        }
    }

    IEnumerator SwordSwing()
    {
        canAttack = false; // Prevent further attacks
        Sword.GetComponent<Animator>().Play("swordswing");
        System.Random random = new System.Random();
        int index = random.Next(swing.Count);
        audioSource.PlayOneShot(swing[index]);
        yield return new WaitForSeconds(attackCooldown); // Wait for the cooldown duration
        Sword.GetComponent<Animator>().Play("idle");
        canAttack = true; // Allow attacks again after cooldown
    }
}