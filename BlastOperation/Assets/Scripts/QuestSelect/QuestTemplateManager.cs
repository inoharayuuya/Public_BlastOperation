using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTemplateManager : MonoBehaviour
{
    ActiveUIManager uiManager;
    QuestAnimationsManager animManager;

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager�擾
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();
        // QuestAnimationsManager�擾
        animManager = GameObject.Find("QuestSelect").GetComponent<QuestAnimationsManager>();

        // �{�^���R���|�[�l���g�擾
        // �{�^���������̊֐��o�^
        this.GetComponent<Button>().onClick.AddListener(uiManager.TapQuest);
        this.GetComponent<Button>().onClick.AddListener(animManager.ListAnActive);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
