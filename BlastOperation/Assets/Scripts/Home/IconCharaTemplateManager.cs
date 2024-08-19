using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCharaTemplateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // RankCardManager�擾
        RankCardManager rcManager = GameObject.Find("RankCardManager").GetComponent<RankCardManager>();

        // �{�^���R���|�[�l���g�擾
        // �{�^���������̊֐��o�^
        this.GetComponent<Button>().onClick.AddListener(()=>rcManager.TapNewCharaIcon(this.gameObject));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
