using System.Collections;
using System.Collections.Generic;
using PlayNANOO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RankingBoard : MonoBehaviour
{
    [SerializeField] private ScrollRect scrRect;
    [SerializeField] private TextMeshProUGUI rankersField;

    // Update is called once per frame
    private void OnEnable()
    {
        rankersField.text = GlobalConst.RankingBoardLoading;
        GetRankingData();
        scrRect.content.localPosition = new Vector3(0, 1, 0);
    }

    private void GetRankingData()
    {
        var rankData = string.Empty;
        var rank = 1;

        GameManager.Instance.nanooPlugin.RankingRange(GlobalConst.RankingTableID, 1, 100,
            (status, errorMessage, jsonString, values) =>
            {
                if (status.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    foreach (Dictionary<string, object> item in (ArrayList) values["items"])
                    {
                        rankData += $"{rank}#{item["nickname"]} - {item["score"]}\n";
                        rank += 1;
                    }

                    for (; rank <= 100; rank += 1)
                    {
                        rankData += $"{rank}#\n";
                    }
                }
                else
                {
                    rankData = errorMessage;
                }

                rankersField.text = rankData;
            }
        );
    }
}
