using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum eAtlasType
{
    UI,
    Block,
    Unit,
    Skill,
    Card,
    Shop,
}

public class AtlasManager : MonoBehaviour
{
    public static AtlasManager Inst;

    [SerializeField] SpriteAtlas ui;
    [SerializeField] SpriteAtlas block;
    [SerializeField] SpriteAtlas unit;
    [SerializeField] SpriteAtlas skill;
    [SerializeField] SpriteAtlas card;
    [SerializeField] SpriteAtlas shop;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    public Sprite GetSprite(eAtlasType type, string name)
    {
        if (type == eAtlasType.Block)
            return block.GetSprite(name);
        else if (type == eAtlasType.Unit)
            return unit.GetSprite(name);
        else if (type == eAtlasType.Skill)
            return skill.GetSprite(name);
        else if (type == eAtlasType.Card)
            return card.GetSprite(name);
        else if (type == eAtlasType.Shop)
            return shop.GetSprite(name);
        else
            return ui.GetSprite(name);
    }
}
