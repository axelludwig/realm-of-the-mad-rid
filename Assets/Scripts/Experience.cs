using UnityEngine;

public class Experience
{
    private int experiencePoints;
    private int level;
    private int experienceForNextLevel;

    public int ExperiencePoints => experiencePoints;
    public int Level => level;
    public int ExperienceForNextLevel => experienceForNextLevel;

    public Experience()
    {
        experiencePoints = 0;
        level = 1;
        experienceForNextLevel = 50;
    }

    public void GainXP(int xp)
    {
        experiencePoints += xp;

        while(experiencePoints >= experienceForNextLevel)
        {
            GainLevel();
        }
    }

    public int GetXPGivenOnDeath()
    {
        return 25 + ((level - 1) * 20);
    }


    /// <summary>
    /// Modifie de force l'xp de l'entit�, sans d�clencher les sideeffects
    /// 
    /// Utilis� pour synchroniser les infos sur l'entit� quand l'xp change c�t� serveur
    /// </summary>
    public void SetXp(int experiencePoints)
    {
        this.experiencePoints = experiencePoints;
    }

    /// <summary>
    /// Modifie de force le level de l'entit�, sans d�clencher le sideeffect
    /// 
    /// Utilis� pour synchroniser les infos sur l'entit� quand l'xp change c�t� serveur
    /// </summary>
    public void SetLevel(int level)
    {
        this.level = level;
    }

    private void GainLevel()
    {
        experiencePoints -= experienceForNextLevel;
        level = level + 1;
        experienceForNextLevel = GetXPNeededForNextLevel();
    }

    private int GetXPNeededForNextLevel()
    {
        return (level * 50 * level/2);
    }
}
