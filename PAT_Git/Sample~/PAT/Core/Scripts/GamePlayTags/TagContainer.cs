using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagContainer : MonoBehaviour
{
    [SerializeField] protected List<GamePlayTag> _inputTags = new();
    [SerializeField] protected List<GamePlayTag> _stateTags= new();
    [SerializeField] protected List<GamePlayTag> _locomotionTags= new();
    [SerializeField] protected List<GamePlayTag> _effectTags= new();
    [SerializeField] protected List<GamePlayTag> _prohibitedTags= new();
    [SerializeField] protected List<GamePlayTag> _generalTags= new();

    #region Getter
    public List<GamePlayTag> stateTags {  get { return _stateTags; } }
    public List<GamePlayTag> locomotionTags { get { return _locomotionTags;} }
    public List<GamePlayTag> effectTags { get { return _effectTags;} }
    public List<GamePlayTag> generalTags { get { return _generalTags; } }
    public List<GamePlayTag> prohibitedTags { get { return _prohibitedTags; } }

    public List<GamePlayTag> inputTags
    {
        get { return _inputTags; }
    }

    #endregion

    
    /// <summary>
    /// This does not check input tags
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool CheckForTag(GamePlayTag t)
    {
        //if (_inputTags.Contains(t)) return true;
        if(_stateTags.Contains(t)) return true;
        if(_locomotionTags.Contains(t)) return true;
        if(_effectTags.Contains(t))return true;
        if(_generalTags.Contains(t)) return true;

        return false;
    }
}
