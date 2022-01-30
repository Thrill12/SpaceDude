using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Items/Weapons/Projectile Gun")]
public class StraightShootingGun : BaseGun
{
    public GameObject projectile;
    public Stat projectileSpeed;

    //A normal shoot function for a straight shooting gun
    public override void Attack(GameObject weaponObject, PlayerInput playerInput, AudioSource audioSource, WeaponsHolder holder)
    {
        base.Attack(weaponObject, playerInput, audioSource, holder);

        if (currentBullets == 0) return;

        Vector3 lookDir = Vector3.zero;
        if(playerInput.currentActionMap.name == "KeyBoard")
        {
            lookDir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        else
        {
            lookDir = weaponObject.transform.up;
        }
        
        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;

        GameObject proj = Instantiate(projectile, shootSource.transform.position, Quaternion.identity);

        //Have to make it relative ofc
        proj.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value + hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * lookDir.normalized);
        proj.GetComponent<Projectile>().entityShotFrom = hostEntity;
        proj.GetComponent<Projectile>().weaponShotFrom = this;

        audioSource.PlayOneShot(attackSound);
        holder.ShakeCamera();
        currentBullets -= 1;
        Destroy(proj, 3);
    }

    public override void GenerateMods()
    {
        base.GenerateMods();

        for (int i = 0; i < Random.Range(1, itemRarity.modAmount + 1); i++)
        {
            int randomProperty = Random.Range(0, 2);

            switch (randomProperty)
            {
                case 0:

                    Modifier mod = new Modifier("Damage", damage, Random.Range(5, 50), Modifier.StatModType.Flat);
                    mod.Source = this;
                    damage.AddModifier(mod);
                    AddMod(mod);

                    break;

                case 1:

                    mod = new Modifier("Attack Cooldown", attackCooldown, Random.Range(-5, -25), Modifier.StatModType.PercentAdd);
                    mod.Source = this;
                    attackCooldown.AddModifier(mod);
                    AddMod(mod);

                    break;

                case 2:

                    mod = new Modifier("Projectile Speed", projectileSpeed, Random.Range(5, 15), Modifier.StatModType.PercentMult);
                    mod.Source = this;
                    projectileSpeed.AddModifier(mod);
                    AddMod(mod);

                    break;
            }
        }        
    }
}
