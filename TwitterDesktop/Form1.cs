using RaaZ_TwitterConnect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tweetinvi;
using Tweetinvi.Parameters;
using TweetSharp;
using Twitterizer;

namespace TwitterDesktop
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            picTweet.AllowDrop = true;
        }

        private XMLStuff myXML;

        private void Form1_Load(object sender, EventArgs e)
        {
            Browser formLogin = new Browser();
            myXML = new XMLStuff();
            lstStatus.Items.Add("Checking for login info..");
            lstStatus.Items.Add("");
            bool loggedIn = AlreadyLoggedIn();
            // statusBarMessage.Text = "Not logged in, shifting to login page.";
            if (!loggedIn)
            {
                lstStatus.Items.Add("Not logged in, shifting to login page.");
                lstStatus.Items.Add("");
                formLogin.Login();
                this.Hide();
                // statusBarMessage.Text = "";
            }
            else
            {
                lstStatus.Items.Add("Login status - Success!");
                lstStatus.Items.Add("");
                Browser mw = new Browser();
                this.Show();
                panLoggedIn.Visible = true;
                loadLogin();
                mw.Close();
                btnLogin.Visible = false;
                btnLogout.Visible = true;
            }
        }

        private void loadLogin()
        {
            lstStatus.Items.Add("Fetching screenname - Success!");
            lstStatus.Items.Add("");
            string screenname = TwitterStuff.screenName;
            lstStatus.Items.Add("Fetching avatar - Success!");
            lstStatus.Items.Add("");
            lstStatus.Items.Add("--------------------------------------------------------");
            lstStatus.Items.Add("");
            lstStatus.Items.Add("Account is now ready.");
            twitterAvatar();
            lblScreenname.Text = screenname;

            List<string> LoginInfo = myXML.readFromXml();

            var service = new TwitterService(TwitterStuff.consumerKey, TwitterStuff.consumerSecret);
            service.AuthenticateWith(LoginInfo[2], LoginInfo[3]);
            var users = service.SearchForUser(new SearchForUserOptions { Q = screenname });
            foreach (var user in users)
            {
                lblFollowers.Text = user.FollowersCount.ToString();
            }
        }

        private void twitterAvatar()
        {
            string screenname = TwitterStuff.screenName;
            string pictureUrl = "https://twitter.com/" + screenname + "/profile_image?size=bigger";
            var request = WebRequest.Create(pictureUrl);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                pictureBox1.Image = Bitmap.FromStream(stream);
            }
        }

        private void SetLocalTokens()
        {
            TwitterStuff.tokens = new OAuthTokens();
            TwitterStuff.tokens.AccessToken = TwitterStuff.tokenResponse2.Token;
            TwitterStuff.tokens.AccessTokenSecret = TwitterStuff.tokenResponse2.TokenSecret;
            TwitterStuff.tokens.ConsumerKey = TwitterStuff.consumerKey;
            TwitterStuff.tokens.ConsumerSecret = TwitterStuff.consumerSecret;
        }

        private bool SetLocalTokens(string accessToken, string tokenSec)
        {
            try
            {
                TwitterStuff.tokens = new OAuthTokens();
                TwitterStuff.tokens.AccessToken = accessToken;
                TwitterStuff.tokens.AccessTokenSecret = tokenSec;
                TwitterStuff.tokens.ConsumerKey = TwitterStuff.consumerKey;
                TwitterStuff.tokens.ConsumerSecret = TwitterStuff.consumerSecret;
                // MessageBox.Show("Tokens with arguments initialized.");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool AlreadyLoggedIn()
        {
            try
            {
                //MessageBox.Show("Trying to get login info");
                List<string> LoginInfo = myXML.readFromXml();

                TwitterStuff.screenName = LoginInfo[1];
                if (!SetLocalTokens(LoginInfo[2], LoginInfo[3]))
                    return false;
                //MessageBox.Show("Already logged in.");
                return true;
            }
            catch (Exception e)
            {
                // statusBarMessage.Text = "Not logged in.";
                return false;
            }
        }

        private void ToolBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            Browser loginForm = new Browser();
            loginForm.ShowDialog();
            this.Hide();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            myXML.Logout();
            MessageBox.Show("User logged out.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void BunifuFlatButton4_Click(object sender, EventArgs e)
        {
            List<string> LoginInfo = myXML.readFromXml();

            Auth.SetUserCredentials(TwitterStuff.consumerKey, TwitterStuff.consumerSecret, LoginInfo[2], LoginInfo[3]);
            string tweetmessage = txtTweet.Text;
            if (picTweet.Image != null)
            {

                byte[] file = File.ReadAllBytes(imagepath);
                var media = Upload.UploadBinary(file);
                try
                {
                    var tweet = Tweet.PublishTweet(tweetmessage, new PublishTweetOptionalParameters
                    {
                        Medias = { media }
                    });
                    
                }
                catch (Exception ex)
                {
                }
                MessageBox.Show("Successfully sent tweet!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //TwitterStuff frmTwitter = new TwitterStuff();
                //frmTwitter.tweetIt(tweetmessage);
                txtTweet.Clear();
                picTweet.Image = null;
            }
            else
            {
                try
                {
                    Tweet.PublishTweet(tweetmessage);
                }
                catch (Exception ex)
                {
                }
                MessageBox.Show("Successfully sent tweet!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //TwitterStuff frmTwitter = new TwitterStuff();
                //frmTwitter.tweetIt(tweetmessage);
                txtTweet.Clear();
            }
            
            
        }

        private void PanLoggedIn_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PicTweet_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        public string imagepath;

        private void PicTweet_DragDrop(object sender, DragEventArgs e)
        {
            foreach (string pic in ((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                Image img = Image.FromFile(pic);
                imagepath = pic;
                picTweet.Image = img;
            }
        }
    }
}