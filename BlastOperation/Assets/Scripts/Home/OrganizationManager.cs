using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrganizationManager : MonoBehaviour
{
    public static GameObject[] orgChara = new GameObject[ActiveUIManager.MAX_ORG];
    [SerializeField] GameObject charaPanelOrgBg;
    [SerializeField] GameObject questOrgBg;

    public static bool isGet;
    public static bool isFirst;

    ActiveUIManager um;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            orgChara[i] = charaPanelOrgBg.transform.GetChild(i).gameObject;
        }
        um = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();

    }

        // Update is called once per frame
        void Update()
        {


        if (isGet)
        {
            if (!isFirst)
            {
                for (int i = 0; i < ActiveUIManager.MAX_ORG; i++)
                {
                    GameObject child = questOrgBg.transform.GetChild(i).gameObject;
                    Destroy(child);
                }

                isFirst = true;
            }


            // 編成中のキャラを取得
            for (int i = 0; i < ActiveUIManager.MAX_ORG; i++)
            {
                GameObject child = charaPanelOrgBg.transform.GetChild(i).gameObject;

                //orgChara[i] = child;
                // プレファブを生成
                var temp = Instantiate(orgChara[i], questOrgBg.transform);

            }

            isGet = false;
            isFirst = false;

            //um.CanvasAnActive();
            }
        }
    }

