using UnityEngine;

public class BlockRemove : FxBase
{
    float index;
    ParticleSystem ps;

    protected override void OnInit(eColor _color)
    {
        index = (int)_color + 1;
        if (ps== null)
            ps = GetComponent<ParticleSystem>();

        var textSheet = ps.textureSheetAnimation;
        textSheet.startFrame = index / 7f;
    }

    protected override void OnEnter()
    {
     
    }
}