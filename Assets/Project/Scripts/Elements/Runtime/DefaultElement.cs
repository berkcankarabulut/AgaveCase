using System;
using AgaveCase.Elements.Runtime;
using UnityEngine;

public class DefaultElement : ElementBase
{
    [Header("Default Element Properties")]
    [SerializeField] protected SpriteRenderer selectedRenderer;
    
    [Header("Animation Settings")] 
    [SerializeField] private float _selectAnimationDuration = 0.2f; 
    [SerializeField] private float _selectAnimationScale = 1.1f;
    [SerializeField] private float _matchHighlightDuration = 0.3f;
    [SerializeField] private float _matchScaleUpFactor = 1.5f;
    [SerializeField] private float _destroyAnimationDuration = 0.4f;
     
    protected ElementAnimationHandler _animationHandler;
    private ElementStateHandler _stateHandler;

    public ElementAnimationHandler AnimationHandler => _animationHandler;

    public override void Initialize(ElementDataSO elementDataSO)
    {
        base.Initialize(elementDataSO);
        
        if (selectedRenderer != null)
        {
            selectedRenderer.enabled = false;
        }
         
        _animationHandler = new ElementAnimationHandler(this, transform, selectedRenderer);
        _stateHandler = new ElementStateHandler(this, selectedRenderer);
         
        AnimationHandler.SetAnimationSettings(
            _selectAnimationDuration,
            _selectAnimationScale,
            _matchHighlightDuration,
            _matchScaleUpFactor,
            _destroyAnimationDuration
        );
    }
     
    public virtual void PlaySpawnAnimation(Vector3 startPosition, Vector3 endPosition, float duration,
        DG.Tweening.Ease easeType, Action onCompleted = null)
    {
        AnimationHandler.PlaySpawnAnimation(startPosition, endPosition, duration, easeType, onCompleted);
    }
    
    public virtual void PlayFallAnimation(Vector3 targetPosition, float duration,
        DG.Tweening.Ease easeType, float delay = 0f, Action onCompleted = null)
    {
        AnimationHandler.PlayFallAnimation(targetPosition, duration, easeType, delay, onCompleted);
    }
     
    public virtual void OnSelected()
    {
        _stateHandler.OnSelected();
    }
    
    public virtual void OnDeselected()
    {
        _stateHandler.OnDeselected();
    }
    
    public virtual void OnMatched()
    {
        _stateHandler.OnMatched();
    }
}