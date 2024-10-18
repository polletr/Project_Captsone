using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealableObject : MonoBehaviour  , IRevealable
{
    private Material objMaterial;
    [SerializeField] private float revealTime;
    [SerializeField] private float unRevealTime;

    public bool IsRevealed
    {
        get => currentObjTransp >= 1f;
        set => currentObjTransp = value ? 1f : origObjTransp;
    }

    private float revealTimer;
    private float currentObjTransp;
    private float origObjTransp;

    void Awake()
    {
        objMaterial = GetComponent<MeshRenderer>().material;
        origObjTransp = objMaterial.GetFloat("_Transparency");
        revealTimer = 0f;

        currentObjTransp = origObjTransp;
    }

    public void ApplyEffect()
    {
        GetComponent<MeshRenderer>().material = objMaterial;
    }

    public void RemoveEffect()
    {

    }

    public void SuddenReveal()
    {
        gameObject.SetActive(true);
        objMaterial.SetFloat("_Transparency", 1f);
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.SuddenAppear, this.transform.position);
    }

    public void RevealObj(out bool revealed)
    {
        StopAllCoroutines();
        revealTimer += Time.deltaTime;
        currentObjTransp = Mathf.Lerp(origObjTransp, 1f, revealTimer / revealTime);
        objMaterial.SetFloat("_Transparency", currentObjTransp);
        if (revealTimer >= revealTime)
        {
            revealTimer = 0f;
        }

        revealed = currentObjTransp >= 1f;
   }

    public void UnRevealObj()
    {
        StartCoroutine(UnReveal());
    }


    private IEnumerator UnReveal()
    {
        revealTimer = 0f;

        var currentFloat = currentObjTransp;
        var timer = 0f;
        while (currentObjTransp > origObjTransp)
        {
            timer += Time.deltaTime;
            currentObjTransp = Mathf.Lerp(currentFloat, origObjTransp, timer / unRevealTime);
            objMaterial.SetFloat("_Transparency", currentObjTransp);

            yield return null;
        }
    }

}
