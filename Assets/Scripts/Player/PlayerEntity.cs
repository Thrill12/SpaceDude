using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : BaseEntity
{
    public Image healthDisplay;
    public Image energyDisplay;

    [Space(5)]

    public float oxygen;
    public Stat maxOxygen;
    public Image oxygenDisplay;

    public bool useOxygen = false;
    private float nextOxygenTick;

    [Space(10)]

    [Header("Player sprites")]

    public SpriteRenderer chestArmourRenderer;
    public SpriteRenderer helmetArmourRenderer;
    public SpriteRenderer backArmourRenderer;

    [Space(10)]

    public Sprite bodyNoHeadSprite;

    public Sprite defaultChestArmourSprite;
    public Sprite defaultChestArmourSmallWeaponEquippedSprite;
    public Sprite defaultChestArmourLargeWeaponEquippedSprite;

    private ItemSlot helmetSlot;
    private ItemSlot chestSlot;
    private ItemSlot backSlot;

    private WeaponsHolder weaponsHolder;   

    public override void Start()
    {
        base.Start();

        oxygen = maxOxygen.Value;

        helmetSlot = GameManager.instance.playerInventory.slots.First(x => x.slotName == "Helmet");
        chestSlot = GameManager.instance.playerInventory.slots.First(x => x.slotName == "Chest");
        backSlot = GameManager.instance.playerInventory.slots.First(x => x.slotName == "Back");

        weaponsHolder = GameManager.instance.playerInventory.weaponHolder;
    }

    public override void Update()
    {
        base.Update();
        healthDisplay.fillAmount = Mathf.Lerp(healthDisplay.fillAmount, health / maxHealth.Value, 0.5f);
        energyDisplay.fillAmount = Mathf.Lerp(energyDisplay.fillAmount, energy / maxEnergy.Value, 0.5f);
        oxygenDisplay.fillAmount = Mathf.Lerp(oxygenDisplay.fillAmount, oxygen / maxOxygen.Value, 0.5f);

        nextOxygenTick -= Time.deltaTime;

        if (nextOxygenTick <= 0)
        {
            if (useOxygen)
            {
                if (oxygen < 0)
                {
                    TakeDamagePure(oxygen);
                    if (!GameManager.instance.uiManager.isWarning)
                    {
                        GameManager.instance.uiManager.ShowWarning("Low Oxygen");
                    }
                }

                oxygen -= 1;
                nextOxygenTick = 1;
            }
            else
            {
                oxygen += 2;
                nextOxygenTick = 0.4f;
            }           
        }

        if (isDead)
        {
            
        }
    }

    public void CheckItemsEquippedSprites()
    {
        if(chestSlot.itemInSlot != null)
        {
            BaseChestArmour chestArmour = chestSlot.itemInSlot as BaseChestArmour;

            if(weaponsHolder.currentlyEquippedWeapon != null)
            {
                if (weaponsHolder.currentlyEquippedWeapon.large)
                {
                    chestArmourRenderer.sprite = chestArmour.spriteLargeWeaponEquipped;
                }
                else
                {
                    chestArmourRenderer.sprite = chestArmour.spriteSmallWeaponEquipped;
                }
            }
            else
            {
                chestArmourRenderer.sprite = chestArmour.spriteNoWeaponsEquipped;
            }
        }
        else
        {
            if (weaponsHolder.currentlyEquippedWeapon != null)
            {
                if (weaponsHolder.currentlyEquippedWeapon.large)
                {
                    chestArmourRenderer.sprite = defaultChestArmourLargeWeaponEquippedSprite;
                }
                else
                {
                    chestArmourRenderer.sprite = defaultChestArmourSmallWeaponEquippedSprite;
                }
            }
            else
            {
                chestArmourRenderer.sprite = defaultChestArmourSprite;
            }
        }

        if (backSlot.itemInSlot != null)
        {
            BaseBackArmour backArmour = backSlot.itemInSlot as BaseBackArmour;

            backArmourRenderer.sprite = backArmour.showableSprite;
        }
        else
        {
            backArmourRenderer.sprite = null;
        }

        if (helmetSlot.itemInSlot != null)
        {
            BaseHelmet helmet = helmetSlot.itemInSlot as BaseHelmet;

            helmetArmourRenderer.sprite = helmet.showableSprite;
        }
        else
        {
            helmetArmourRenderer.sprite = null;
        }
    }

    public override void OnKill(BaseEntity victim, BaseEntity killer)
    {
        base.OnKill(victim, killer);

        int xpToGive = (int)Mathf.Ceil(victim.maxHealth.Value / GameManager.instance.playerInventory.itemsEquipped.Count());

        foreach (var item in GameManager.instance.playerInventory.itemsEquipped)
        {
            item.AddXP(xpToGive);
        }
    }

    public override void Die(BaseEntity killer)
    {
        isDead = true;

        float mixerEQ = 0;
        GameManager.instance.masterMixer.GetFloat("EQMaster", out mixerEQ);
        LeanTween.value(gameObject, mixerEQ, 0.05f, 2).setOnUpdate((float val) => GameManager.instance.masterMixer.SetFloat("EQMaster", val)).setIgnoreTimeScale(true);
        LeanTween.value(gameObject, 1, 0, 2).setOnUpdate((float val) => Time.timeScale = val).setIgnoreTimeScale(true);

        GameManager.instance.uiManager.DisplayDeathScreen();
    }
}
