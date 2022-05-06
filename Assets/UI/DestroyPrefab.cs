using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECAScripts;
using UnityEngine;

public class DestroyPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void destroyPrefab()
    {
        Debug.Log("destroying " + gameObject.name);
        if(gameObject.name.Equals("SimpleConditionPrefab(Clone)"))
        {
            //Remove the color from the gameobject selected
            ConditionDropdownHandler conditionDropdownHandler = gameObject.GetComponent<ConditionDropdownHandler>();
            if(conditionDropdownHandler.ToCheckSelected && 
               conditionDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>())
                Destroy(conditionDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>());
            var allConditionsParentObj = GameObject.FindGameObjectsWithTag("CompositeCondition").ToList();
            var conditionsParentObj = from act in allConditionsParentObj where act.name != "CompositeConditionPrefab" select act;
            if(conditionsParentObj.Count() > 0)
            {
                foreach (var condition in conditionsParentObj)
                {
                    ConditionDropdownHandler compositeDropdownHandler = condition.GetComponent<ConditionDropdownHandler>();
                    if(compositeDropdownHandler.ToCheckSelected && 
                       compositeDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>())
                        Destroy(compositeDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>());
                    Destroy(condition);//destroying clones
                }
            }
            GameObject.Find("ConditionList").SetActive(false);
            GameObject.Find("_headerCondition").SetActive(false);
        }
        Destroy(this.gameObject);
    }
}
