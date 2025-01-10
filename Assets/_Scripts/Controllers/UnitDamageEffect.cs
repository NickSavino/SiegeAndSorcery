using UnityEngine;

public class UnitDamageEffect
{

    SpriteRenderer _sprite;
    private float ATTACK_FLASH_TIME = 0.2f;        // should probably be contained in an enum for other damage effects later
    private Color DEFAULT_SPRITE_COLOR = Color.white;
    private Color DAMAGE_SPRITE_COLOR = Color.red;

    private float _damageFlashTime;

    public UnitDamageEffect(SpriteRenderer sprite)
    {
        this._sprite = sprite;
        _damageFlashTime = -1f;     // inactive, no damage effect happening
    }



    public void StartDamageEffect()
    {
        _sprite.color = DAMAGE_SPRITE_COLOR;
        _damageFlashTime = 0f;
    }

    public void UpdateTakeDamageTime()
    {
        if (_damageFlashTime >= 0f)     // only do this if an effect is active (inactive = -1f)
        {
            _damageFlashTime += Time.deltaTime;
            if (_damageFlashTime > ATTACK_FLASH_TIME)
            {
                _sprite.color = DEFAULT_SPRITE_COLOR;
                _damageFlashTime = -1f;         // if time has passed, finish effect
            }
        }

    }


}
