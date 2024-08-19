using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrgCharaTemplate : MonoBehaviour
{
    ActiveUIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager�擾
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();

        // �{�^���R���|�[�l���g�擾
        // �{�^���������̊֐��o�^
        this.GetComponent<Button>().onClick.AddListener(() => uiManager.TapOrgChara(this.gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
