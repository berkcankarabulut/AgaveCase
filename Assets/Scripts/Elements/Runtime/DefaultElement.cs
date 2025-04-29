using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    public class DefaultElement : ElementBase
    { 
        private float _spawnTime;
        private bool _isActive = false;
        
        public override void OnGetFromPool()
        {
            base.OnGetFromPool();  
            _spawnTime = Time.time;
            _isActive = true;

            if (elementData == null) return;
            if (elementData.SpawnEffectPrefab != null)
            {
                Instantiate(elementData.SpawnEffectPrefab, transform.position, Quaternion.identity);
            } 
        } 
        
        public override void OnReturnToPool()
        { 
            if (elementData != null && _isActive)
            { 
                if (elementData.DestroyEffectPrefab != null)
                {
                    Instantiate(elementData.DestroyEffectPrefab, transform.position, Quaternion.identity);
                } 
            }
            
            _isActive = false;
            base.OnReturnToPool();
        } 
        
        public void ApplyMovement(Vector2 direction)
        {
            if (!_isActive || elementData == null) return;
            
            float speed = elementData.BaseSpeed; 
            transform.Translate(direction.normalized * speed * Time.deltaTime); 
        }
    }
}