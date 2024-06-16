using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Ressources : NetworkBehaviour {

    [SerializeField] TextMeshProUGUI goldCountText;
    [SerializeField] Image expBarImage;
    [SerializeField] TextMeshProUGUI expProgressText;
    [SerializeField] TextMeshProUGUI lvlText;

    public int goldCount = 0;
    public int expAmount = 0;
    public int expTreshhold = 10;
    public int lvl = 0;
    

    private const string PLAYER_PREFS_GOLD = "Gold";
    private const string PLAYER_PREFS_EXP = "Exp";
    private const string PLAYER_PREFS_LVL = "Lvl";

    private void Start()
    {
        PlayerPrefs.SetInt(PLAYER_PREFS_GOLD, 0);
        PlayerPrefs.SetInt(PLAYER_PREFS_EXP, 0);
        PlayerPrefs.SetInt(PLAYER_PREFS_LVL, 0);
        PlayerPrefs.Save();
        if (IsOwner)
        {
            LoadPlayerPrefs();
            UpdateUI();
        }
        
    }

    private void Awake()
    {
        if (IsOwner)
        {
            LoadPlayerPrefs();
        }
    }
    public void Update(){
        if (Input.GetKeyDown(KeyCode.O)){
            goldCount = 0;
            expAmount = 0;
            expTreshhold = 10;
            lvl = 0;
            UpdateUI();
        }
        UpdateUI();
    }

    private void LoadPlayerPrefs()
    {
        goldCount = PlayerPrefs.GetInt(PLAYER_PREFS_GOLD, 0);
        expAmount = PlayerPrefs.GetInt(PLAYER_PREFS_EXP, 0);
        lvl = PlayerPrefs.GetInt(PLAYER_PREFS_LVL, 0);
        expTreshhold = 10 * (1 << lvl);
    }

    private void UpdateUI()
    {
        expBarImage.fillAmount = (float)expAmount / expTreshhold;
        goldCountText.text = goldCount.ToString();
        lvlText.text = $"Lvl {lvl}";
        expProgressText.text = $"{expAmount} / {expTreshhold}";
    }

    public void UpdateRessources(int gold, int exp)
    {
        if (!IsOwner) return;

        goldCount += gold;
        expAmount += exp;

        while (expAmount > expTreshhold)
        {
            lvl += 1;
            expAmount -= expTreshhold;
            expTreshhold *= 2;
        }

        UpdateUI();

        PlayerPrefs.SetInt(PLAYER_PREFS_GOLD, goldCount);
        PlayerPrefs.SetInt(PLAYER_PREFS_EXP, expAmount);
        PlayerPrefs.SetInt(PLAYER_PREFS_LVL, lvl);
        PlayerPrefs.Save();

        UpdateRessourcesServerRpc(goldCount, expAmount, lvl);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateRessourcesServerRpc(int gold, int exp, int level)
    {
        goldCount = gold;
        expAmount = exp;
        lvl = level;
        expTreshhold = 10 * (1 << lvl);

        UpdateUI();
        UpdateRessourcesClientRpc(gold, exp, level);
        Debug.Log("XP and gold added");
    }

    [ClientRpc]
    public void UpdateRessourcesClientRpc(int gold, int exp, int level)
    {
        if (!IsOwner)
        {
            goldCount = gold;
            expAmount = exp;
            lvl = level;
            expTreshhold = 10 * (1 << lvl);

            UpdateUI();
        }

        Debug.Log("XP and gold added");
    }

    public void Pay(int gold){
        goldCount -= gold;
        PlayerPrefs.SetInt(PLAYER_PREFS_GOLD, goldCount);
        PlayerPrefs.Save();        
    }
}