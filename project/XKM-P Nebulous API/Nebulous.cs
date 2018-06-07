using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Nebulous
{
    /*
     * Nebulous Account API developed by XKM-P, 2018.
     * https://github.com/xkm-p
     * Licensed under the Apache License Version 2.0
     */

    public static class AccountAPI
    {
        static String LoginTicket = "EMPTY";

        /// <summary>
        /// Set the login ticket. See the README.md file for information on how to obtain the ticket.
        /// </summary>
        /// <param name="LoginTicket"></param>
        public static void SetLoginTicket(String LoginTicket)
        {
            AccountAPI.LoginTicket = LoginTicket;
        }

        /// <summary>
        /// Sets the BFF (best friend) status of a friend.
        /// </summary>
        /// <param name="AccountID">Account ID of the friend</param>
        /// <param name="IsBFF">New BFF status</param>
        /// <param name="IsForClan">Apply to clan list (false for friends list)</param>
        public static void SetBFF(String AccountID, bool IsBFF, bool IsForClan)
        {
            APICall("BFF", "AID=" + AccountID + "&BFF=" + IsBFF + "&ForClan=" + IsForClan);
        }

        /// <summary>
        /// Mark a mail as unread.
        /// </summary>
        /// <param name="MsgID">The ID of the mail</param>
        public static void MarkMailUnread(String MsgID)
        {
            APICall("MarkMailUnread", "msgID=" + MsgID);
        }

        /// <summary>
        /// Get details for a specific mail.
        /// </summary>
        /// <param name="MsgID">The ID of the mail</param>
        /// <returns></returns>
        public static ReadMailObject GetMail(String MsgID)
        {
            String jsonString = APICall("ReadMail", "MsgID=" + MsgID);
            if (jsonString == null)
                return null;
            ReadMailObject obj = JsonConvert.DeserializeObject<ReadMailObject>(jsonString);
            return obj;
        }

        /// <summary>
        /// Delete a mail.
        /// </summary>
        /// <param name="MsgID">The ID of the mail</param>
        /// <param name="Received">Set to true if the mail was received, false if the mail was sent</param>
        public static void DeleteMail(String MsgID, bool Received)
        {
            APICall("DeleteMail", "MsgID=" + MsgID + "&Received=" + Received);
        }

        /// <summary>
        /// Send a mail to an account.
        /// </summary>
        /// <param name="ToAID">ID of any account</param>
        /// <param name="Subject">The subject of the mail</param>
        /// <param name="Message">The actual message</param>
        /// <param name="ToAllClanMembers">Send it to all clan members. ToAID will be overriden.</param>
        public static void SendMail(String ToAID, String Subject, String Message, bool ToAllClanMembers)
        {
            APICall("SendMail", "ToAID=" + ToAID + "&ToAllClan=" + ToAllClanMembers + "&Subject=" + HttpUtility.UrlEncode(Subject) + "&Message=" + HttpUtility.UrlEncode(Message));
        }

        /// <summary>
        /// Get statistics for an account, including account name and how much plasma the player has.
        /// </summary>
        /// <param name="AccountID">ID of any account</param>
        /// <param name="GameMode">The gamemode, default: ALL</param>
        /// <returns></returns>
        public static PlayerStatsObject GetStats(String AccountID, String GameMode = "ALL")
        {
            String jsonString = APICall("GetPlayerStats", "AccountId=" + AccountID + "&GameMode=" + GameMode);
            if (jsonString == null)
                return null;
            PlayerStatsObject obj = JsonConvert.DeserializeObject<PlayerStatsObject>(jsonString);
            return obj;
        }

        /// <summary>
        /// Get the profile for a given ID.
        /// </summary>
        /// <param name="Id">ID of any account</param>
        /// <returns></returns>
        public static Profile GetPlayerProfile(String Id)
        {
            String jsonString = APICall("GetPlayerProfile", "accountID=" + Id);
            if (jsonString == null)
                return null;
            Profile obj = JsonConvert.DeserializeObject<Profile>(jsonString);
            return obj;
        }

        /// <summary>
        /// Get a list of all received mails.
        /// </summary>
        /// <returns></returns>
        public static MailCollection GetReceivedMails()
        {
            String jsonString = APICall("GetMailList", "Received=True");
            if (jsonString == null)
                return null;
            MailCollection obj = JsonConvert.DeserializeObject<MailCollection>(jsonString);
            return obj;
        }

        /// <summary>
        /// Get a list of all sent mails.
        /// </summary>
        /// <returns></returns>
        public static MailCollection GetSentMails()
        {
            String jsonString = APICall("GetMailList", "Received=False");
            if (jsonString == null)
                return null;
            MailCollection obj = JsonConvert.DeserializeObject<MailCollection>(jsonString);
            return obj;
        }

        /// <summary>
        /// Get a list of all friends.
        /// </summary>
        /// <param name="StartIndex">Set the startindex for the list</param>
        /// <param name="Count">Set the amount of friends to retrieve (if the entered number is higher than the amount of friends, only that amount will be returned)</param>
        /// <returns></returns>
        public static Friend GetFriends(int StartIndex, int Count)
        {
            String jsonString = APICall("GetFriends", "StartIndex=" + StartIndex + "&Count=" + Count);
            if (jsonString == null)
                return null;
            Friend obj = JsonConvert.DeserializeObject<Friend>(jsonString);
            return obj;
        }

        /// <summary>
        /// Get mail alerts.
        /// </summary>
        /// <returns></returns>
        public static MailAlert GetMailAlerts()
        {
            String jsonString = APICall("GetMailAlerts", "");
            if (jsonString == null)
                return null;
            MailAlert obj = JsonConvert.DeserializeObject<MailAlert>(jsonString);
            return obj;
        }

        /// <summary>
        /// Direct API Call function. Should not be called unless for experimental reasons.
        /// </summary>
        /// <param name="APIFunction">The API function to call</param>
        /// <param name="additionalPostData">Additional POST data</param>
        /// <returns></returns>
        public static String APICall(String APIFunction, String additionalPostData)
        {
            if (LoginTicket.Equals("EMPTY"))
                throw new Exception("Invalid LoginTicket.");

            try
            {
                // Create WebRequest
                var request = (HttpWebRequest)WebRequest.Create("https://www.simplicialsoftware.com/api/account/" + APIFunction);

                // Predefined PostData
                var postData = "Game=Nebulous&Version=447&Ticket=" + HttpUtility.UrlEncode(LoginTicket) + "&" + additionalPostData;
                var data = Encoding.ASCII.GetBytes(postData);

                // Send over POST
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to execute API call. Check your Internet connection.", ex);
            }
        }
    }

    /*
     * Classes for parsing the JSON string
     */
     
    public class PurchasedPet
    {
        public long ID { get; set; }
        public long XP { get; set; }
    }

    public class SpecialObject
    {
        public string Type { get; set; }
        public long Count { get; set; }
    }

    // Player info
    public class PlayerStatsObject
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public List<object> AccountColors { get; set; }
        public bool CanStartClanWar { get; set; }
        public bool CanJoinClanWar { get; set; }
        public bool CanUploadClanSkin { get; set; }
        public bool CanSetMOTD { get; set; }
        public long DQ { get; set; }
        public bool DQDone { get; set; }
        public long XPMultiplier { get; set; }
        public long XPMultiplierDurationRemainingS { get; set; }
        public string ClickType { get; set; }
        public long ClickDurationS { get; set; }
        public long LengthBoost { get; set; }
        public long LengthBoostDurationRemainingS { get; set; }
        public List<long> PurchasedAvatars { get; set; }
        public List<long> PurchasedEjectSkins { get; set; }
        public List<long> PurchasedHats { get; set; }
        public List<PurchasedPet> PurchasedPets { get; set; }
        public List<long> ValidCustomSkinIDs { get; set; }
        public List<long> ValidCustomPetSkinIDs { get; set; }
        public List<object> ValidCustomParticleIDs { get; set; }
        public bool PurchasedAliasColors { get; set; }
        public bool PurchasedClanColors { get; set; }
        public bool PurchasedBlobColor { get; set; }
        public bool clickEnabled { get; set; }
        public long XP { get; set; }
        public long DotsEaten { get; set; }
        public long BlobsEaten { get; set; }
        public long BlobsLost { get; set; }
        public long BiggestBlob { get; set; }
        public long MassGained { get; set; }
        public long MassEjected { get; set; }
        public long EjectCount { get; set; }
        public long SplitCount { get; set; }
        public long AverageScore { get; set; }
        public long HighestScore { get; set; }
        public long TimesRestarted { get; set; }
        public long LongestLifeMS { get; set; }
        public long GamesWon { get; set; }
        public long SMBHCollidedCount { get; set; }
        public long SMBHEatenCount { get; set; }
        public long BHCollidedCount { get; set; }
        public long ArenasWon { get; set; }
        public long CWsWon { get; set; }
        public long TBHCollidedCount { get; set; }
        public long TimesTeleported { get; set; }
        public long PowerupsUsed { get; set; }
        public long MatchesWon { get; set; }
        public List<long> AchievementsEarned { get; set; }
        public List<object> AchievementStats { get; set; }
        public List<SpecialObject> SpecialObjects { get; set; }
        public long CoinsCollected { get; set; }
        public long CurrentCoins { get; set; }
        public bool PurchasedSkinMap { get; set; }
        public bool purchasedSecondPet { get; set; }
        public long trianglesDestroyed { get; set; }
        public long squaresDestroyed { get; set; }
        public long pentagonsDestroyed { get; set; }
        public long hexagonsDestroyed { get; set; }
        public long playersKilled { get; set; }
        public long shotsFired { get; set; }
        public double damageDealt { get; set; }
        public double damageTaken { get; set; }
        public double damageHealed { get; set; }
        public long clanID { get; set; }
        public string ClanName { get; set; }
        public List<long> ClanColors { get; set; }
        public string ClanRole { get; set; }
        public long ClanCoins { get; set; }
        public string EffectiveClanRole { get; set; }
        public bool CanSelfPromote { get; set; }
        public object Error { get; set; }
    }

    public class FriendRequest
    {
        public DateTime LastPlayedUtc { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<object> AccountColors { get; set; }
        public long XP { get; set; }
        public string Relationship { get; set; }
        public string ClanName { get; set; }
        public string ClanRole { get; set; }
        public List<object> ClanColors { get; set; }
        public bool BFF { get; set; }
    }

    public class Friend
    {
        public List<FriendRequest> FriendRequests { get; set; }
        public long totalFriends { get; set; }
        public object Error { get; set; }
    }

    public class MailAlert
    {
        public bool NewMail { get; set; }
        public object Error { get; set; }
    }

    public class Mail
    {
        public long MsgID { get; set; }
        public long FromAID { get; set; }
        public long ToAID { get; set; }
        public string ToName { get; set; }
        public List<long> ToColors { get; set; }
        public string FromName { get; set; }
        public List<object> FromColors { get; set; }
        public string Subject { get; set; }
        public bool IsNew { get; set; }
        public DateTime TimeSent { get; set; }
        public DateTime TimeExpires { get; set; }
    }

    public class MailCollection
    {
        public List<Mail> Mail { get; set; }
        public bool mailNotifications { get; set; }
        public object Error { get; set; }
    }

    public class Profile
    {
        public string profile { get; set; }
        public long customSkinID { get; set; }
        public long setNamePrice { get; set; }
        public bool banned { get; set; }
        public object Error { get; set; }
    }

    public class ReadMailObject
    {
        public long MsgID { get; set; }
        public long FromAID { get; set; }
        public long ToAID { get; set; }
        public string ToName { get; set; }
        public List<long> ToColors { get; set; }
        public string FromName { get; set; }
        public List<object> FromColors { get; set; }
        public string Subject { get; set; }
        public bool IsNew { get; set; }
        public DateTime TimeSent { get; set; }
        public DateTime TimeExpires { get; set; }
        public string Message { get; set; }
        public object Error { get; set; }
    }
}
