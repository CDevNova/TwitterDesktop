using System;
using System.Collections.Generic;
using System.Windows;
using Twitterizer;

namespace RaaZ_TwitterConnect
{
    internal class TwitterStuff
    {
        public static string consumerKey = "consumerkey";
        public static string consumerSecret = "consumersecret";
        public static string pin = "";
        public static string callbackAddy = "oob";
        public static string screenName;

        public static OAuthTokenResponse tokenResponse;
        public static OAuthTokenResponse tokenResponse2;

        public static OAuthTokens tokens;

        public void SendMessage(string msg, string receiverScreenName)
        {
            try
            {
                TwitterDirectMessage.Send(tokens, receiverScreenName.Trim(), msg);
                MessageBox.Show(msg + " to " + receiverScreenName + " has been sent successfuly");
            }
            catch (TwitterizerException te)
            {
                MessageBox.Show(te.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool tweetIt(string status)
        {
            try
            {
                TwitterStatus.Update(tokens, status);
                MessageBox.Show("Tweet sent!");
                return true;
            }
            catch (TwitterizerException te)
            {
                MessageBox.Show(te.Message);
                return false;
            }
        }

        public List<string> getMessages()
        {
            List<string> temp = new List<string>();
            var receivedMessages = TwitterDirectMessage.DirectMessages(tokens);
            foreach (var v in receivedMessages.ResponseObject)
            {
                temp.Add(v.Recipient.ScreenName + " >> " + v.Text);
            }

            return temp;
        }
    }
}