using UnityEngine;

public static class GlobalConst
{
        public const string RankingTableID = "ud0724dodgemaster-RANK-754EEABC-9DC25100";
        public const string AdBannerAndroidUnitID = "ca-app-pub-2277179007654067/3672888252";
        public const string AdInterstitialAndroidUnitID = "ca-app-pub-2277179007654067/4229661146";

        public const string FileNameUserData = "uData";
        public static readonly string FilePathUserData = Application.persistentDataPath + "/" + FileNameUserData;

        #region testAsset

        
        public const string RankingBoardLoading = "Loading...";
        public const string UnknownNickname = "unknown";

        public const string ErrorMsgAccount = "<#ff9999>Failed to generate userID...</color>\n If you want leave ranking properly, restart game or play in a network environment.";
        public const string ErrorMsgNetworkErr = "<#ff9999>Network Error</color>\n Cannot connect to server. Your max score stroed in local directoy. When you playing in network enviroment, the highest record will send to ranking server.";
        public const string ErrorMsgPurchase = "<#ff9999>Failed to purchase.</color>\n";
        public const string ErrorMsgRankingLoadFail = "<#ff9999>Failed to load ranking data</color>\n";
        public const string ErrorMsgRestoreFail = "<#ff9999>Purchase restore failed</color> If you have any purchase and want restore them, please restart game.\n";

        #endregion
}