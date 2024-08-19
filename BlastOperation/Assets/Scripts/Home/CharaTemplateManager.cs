using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharaTemplateManager : MonoBehaviour
{
    ActiveUIManager uiManager;

    // �L�����̉摜
    public static Sprite cSprite;

    private void AddEventTrigger(EventTriggerType _type, Action _event)
    {
        // �C�x���g�g���K�[�R���|�[�l���g�擾
        // �{�^�����������̊֐��o�^
        EventTrigger currentTrigger = this.GetComponent<EventTrigger>(); // �C�x���g�g���K�[�擾
     
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = _type;   //PointerUp�C�x���g�ɒǉ�

        entry.callback.AddListener((eventData)=> { _event(); });   //�Ăяo���֐��̓o�^

        currentTrigger.triggers.Add(entry);                              // �֐��̓o�^

    }

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager�擾
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();

        // �{�^���R���|�[�l���g�擾
        // �{�^���������̊֐��o�^
        this.GetComponent<Button>().onClick.AddListener(uiManager.TapChara);
        this.GetComponent<Button>().onClick.AddListener(TapChara2);



        AddEventTrigger(EventTriggerType.PointerDown,uiManager.PointerDownChara);
        AddEventTrigger(EventTriggerType.PointerUp, uiManager.PointerUpChara);

    } 

    public void TapChara2()
    {
        
        // �ʏ�Ґ����[�h
        if (uiManager.isOrg)
        {
            OrganizationManager.isGet = true;
            OrganizationManager.isFirst = false;

            // �L�����̉摜���擾
            cSprite = GetComponent<Image>().sprite;

            uiManager.orgChara.GetComponent<Image>().sprite = cSprite;
            
            // �ʏ�Ґ��ŕҐ������L�����ƈꊇ�Ґ��̎��ɕ\������L���������ɂ���
            

            //this.gameObject.GetComponent<Image>().color = new Color(0,0,0);
        }
        
        
        // �ꊇ�Ґ����̃L�������^�b�v������O��
        if(this.gameObject.tag == "Bulk")
        {
            //uiManager.isBulkOrg = false;
            uiManager.bulkCnt--;

            // �Ґ�����O�����Ƃ��̏������


            Debug.Log("�ꊇ�J�E���g : " + uiManager.bulkCnt);
            Destroy(gameObject);

        }
        else
        {
            if (uiManager.isBulkOrg)
            {
                uiManager.AddBulkOrg(this.gameObject);
            }
        }


    }

}
