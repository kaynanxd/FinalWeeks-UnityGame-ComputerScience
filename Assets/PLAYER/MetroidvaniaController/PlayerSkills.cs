using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance;

    [Header("Habilidades Desbloqueadas")]
    public bool canDoubleJump = false;
    public bool canWallClimb = false;
    public bool canAttack = false;
    public bool canDash = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockSkill(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.DoubleJump: canDoubleJump = true; break;
            case SkillType.WallClimb: canWallClimb = true; break;
            case SkillType.Attack: canAttack = true; break;
            case SkillType.Dash: canDash = true; break;
        }
    }
}

// Enum para facilitar a seleção no Inspector
public enum SkillType
{
    DoubleJump,
    WallClimb,
    Attack,
    Dash
}