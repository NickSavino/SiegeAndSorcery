using System;
using UnityEngine;

public class StructureDamageEffect
{

    MeshRenderer _mesh;
    MeshRenderer[] _childMeshes;
    private float ATTACK_FLASH_TIME = 0.1f;        // should probably be contained in an enum for other damage effects later
    private Color DEFAULT_SPRITE_COLOR = Color.white;
    private Color DAMAGE_SPRITE_COLOR = Color.red;
    private Color _initialColor;

    private float _damageFlashTime;

    public StructureDamageEffect(MeshRenderer mesh, MeshRenderer[] childMeshes)
    {
        _childMeshes = childMeshes;

        _mesh = mesh;
        if (_mesh != null)
        {
            _initialColor = mesh.material.color;
        }
        else
        {
            _initialColor = _childMeshes[0].material.color;
        }



        _damageFlashTime = -1f;     // inactive, no damage effect happening
    }



    public void StartDamageEffect()
    {
        if (_mesh != null)      // if parent object has mesh renderer
        {
            _mesh.material.color = DAMAGE_SPRITE_COLOR;
        }
        foreach (MeshRenderer child in _childMeshes)    // if child meshes exist
        {
            child.material.color = DAMAGE_SPRITE_COLOR;
        }
        
        _damageFlashTime = 0f;
    }

    public void UpdateTakeDamageTime()
    {
        if (_damageFlashTime >= 0f)     // only do this if an effect is active (inactive = -1f)
        {
            _damageFlashTime += Time.deltaTime;
            if (_damageFlashTime > ATTACK_FLASH_TIME)
            {
                if (_mesh != null)
                {
                    _mesh.material.color = _initialColor;
                }
                foreach (MeshRenderer child in _childMeshes) {
               
                    child.material.color = _initialColor;
                }
                _damageFlashTime = -1f;         // if time has passed, finish effect
            }
        }

    }


}
