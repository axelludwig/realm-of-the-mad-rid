using UnityEngine;

public class Experience
{
    private Entity entity;

    private int experiencePoints;
    private int level;

    public int ExperiencePoints => experiencePoints;
    public int Level => level;
    public int ExperienceForNextLevel => GetXPNeededForNextLevel();

    public Experience(Entity entity)
    {
        experiencePoints = 0;
        level = 1;
        this.entity = entity;
    }

    public void GainXP(int xp)
    {
        experiencePoints += xp;

        while(experiencePoints >= GetXPNeededForNextLevel())
        {
            GainLevel();
        }
    }

    public int GetXPGivenOnDeath()
    {
        return 25 + ((level - 1) * 20);
    }


    /// <summary>
    /// Modifie de force l'xp de l'entité, sans déclencher les sideeffects
    /// 
    /// Utilisé pour synchroniser les infos sur l'entité quand l'xp change côté serveur
    /// </summary>
    public void SetXp(int experiencePoints)
    {
        this.experiencePoints = experiencePoints;
    }

    /// <summary>
    /// Modifie de force le level de l'entité, sans déclencher le sideeffect
    /// 
    /// Utilisé pour synchroniser les infos sur l'entité quand l'xp change côté serveur
    /// </summary>
    public void SetLevel(int level)
    {
        this.level = level;
    }

    private void GainLevel()
    {
        experiencePoints -= GetXPNeededForNextLevel();
        level = level + 1;
    }

    public int GetXPNeededForNextLevel()
    {
        if (level == 1)
            return 50;

        return (level * 50 * level/2);
    }
}
