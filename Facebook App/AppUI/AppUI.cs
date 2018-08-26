using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AppLogic;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;
using System.Threading;
using System.Reflection;

namespace DP_EX1
{
    public partial class AppUI : Form
    {
        private ApplicationLogic m_AppLogic;
        private Dictionary<string, Operation> m_Operations;
        public LinkedList<Album> Albums { get; set; }

        public AppUI()
        {
            InitializeComponent();
            m_AppLogic = new ApplicationLogic();
            m_Operations = new Dictionary<string, Operation>()
            {
                {"LoginBtn", new Operation() { Command = login } },
                {"ShowLikesListBtn", new Operation() {Command = fetchLikesPerFriendList } },
                {"ButtonFirstLiker", new Operation() {Command = fetchFirstLikersToPicsList } },
                {"ButtonPost", new Operation() {Command= postStatus } },
                {"ButtonFetchFriends", new Operation() {Command = fetchFriendsList } },
                {"ButtonFetchAlbums", new Operation() {Command = fetchAlbums } }
            };

            FacebookService.s_CollectionLimit = 200;
            FacebookService.s_FbApiVersion = 2.8f;
            this.Text = "App Login";
            comboBoxSort.SelectedIndexChanged += ComboBoxSort_SelectedIndexChanged;
        }

        private void doWhenButtonClicked(object sender, EventArgs e)
        {
            Button senderBtn = sender as Button;
            if (senderBtn.Name == "LoginBtn")
            {
                if (m_AppLogic.LoginSuccess())
                {
                    postBindingSource.DataSource = m_AppLogic.LoggedInUser.Posts;
                    new Thread(() => m_Operations["LoginBtn"].Execute()).Start();
                }
                else
                {
                    MessageBox.Show("Login failed");
                }
            }
            else
            {
                new Thread(() => m_Operations[senderBtn.Name].Execute()).Start();
            }
        }

        private void ComboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortingAttribute t = comboBoxSort.SelectedItem as SortingAttribute;

            Type type = this.GetType();
            IEnumerable<MethodInfo> info = type.GetRuntimeMethods();
            foreach (MethodInfo method in info)
            {
                Object[] attr = method.GetCustomAttributes(true);
                foreach (var k in attr)
                {
                    if (k is SortingAttribute)
                    {
                        SortingAttribute s = k as SortingAttribute;
                        if (t.Name == s.Name)
                        {
                            method.Invoke(this, null);

                        }
                    }
                }
            }
        }

        private void login()
        {
            UserPictureBox.Invoke(new Action
                   (() => UserPictureBox.LoadAsync(m_AppLogic.LoggedInUser.PictureNormalURL)));
            ButtonFirstLiker.Invoke(new Action ( ()=> ButtonFirstLiker.Enabled = true)) ;
            ShowLikesListBtn.Invoke(new Action(() => ShowLikesListBtn.Enabled = true));
            ButtonFetchFriends.Invoke(new Action(() => ButtonFetchFriends.Enabled = true));
            ButtonFetchAlbums.Invoke(new Action(() => ButtonFetchAlbums.Enabled = true));
            ButtonPost.Invoke(new Action(() => ButtonPost.Enabled = true));
            this.Invoke(new Action(() => this.Text = "Logged in as: " + m_AppLogic.LoggedInUser.Name));          
            this.Invoke(new Action(() => this.listBoxAlbums.SelectedIndexChanged += ListBoxAlbums_SelectedIndexChanged));
        }

        private void fetchLikesPerFriendList()
        {
            LikesListBox.Invoke(new Action(() => LikesListBox.Items.Clear()));
            m_AppLogic.LikesPerFriendBuilder.BuildFeature();
            foreach (UserProxy user in m_AppLogic.LikesPerFriendBuilder)
            {
                LikesListBox.Invoke(new Action(() => LikesListBox.Items.Add(user)));
            }
        }

        private void fetchFirstLikersToPicsList()
        {
            FirstToLikeListBox.Invoke(new Action(() =>
            {
                FirstToLikeListBox.Items.Clear(); 
                FirstToLikeListBox.DisplayMember = "Name";
            }));
            m_AppLogic.FirstLikersBuilder.BuildFeature();
            foreach (UserProxy user in m_AppLogic.FirstLikersBuilder)
            {
                FirstToLikeListBox.Invoke(new Action(() =>
                FirstToLikeListBox.Items.Add(user.RealUser)));
            }
        }

        private void ListBoxAlbums_SelectedIndexChanged(object sender, EventArgs e)
        {
            int top = 3;
            panelAlbumPhotos.Controls.Clear();
            Album a = listBoxAlbums.SelectedItem as Album;   
            foreach (Photo photo in a.Photos)
            {
                LazyPictureBox albumPhoto = new LazyPictureBox();
                albumPhoto.Left = 3;
                albumPhoto.Top = top;
                albumPhoto.Size = new Size(115, 115);
                albumPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                albumPhoto.Load(photo.PictureNormalURL);
                panelAlbumPhotos.Controls.Add(albumPhoto);
                top = albumPhoto.Bottom + 3;
            }
        }

        private void postStatus()
        {
            if (string.IsNullOrEmpty(PostTextBox.Text))
            {
                MessageBox.Show("There is nothing to post, please try again");
            }
            else
            {
                m_AppLogic.PostStatus(PostTextBox.Text);
                PostTextBox.Invoke(new Action(()=>PostTextBox.Clear()));
            }
        }

        private void DoWhenUserSelcted(object sender, EventArgs e)
        {
            ListBox sourceListBox = sender as ListBox;
            User userSelcted = sourceListBox.SelectedItem as User;
            UserProxy userProxySelected = sourceListBox.SelectedItem as UserProxy;
            if (userSelcted != null)
            {
                updateFriendDetails(userSelcted);
            } 
            else if (userProxySelected != null)
            {
                updateFriendDetails(userProxySelected.RealUser);
            }
        }

        private void updateFriendDetails(User i_SelectedUser)
        {
            if (i_SelectedUser != null)
            {
               FriendPictureBox.LoadAsync(i_SelectedUser.PictureNormalURL);
               NameTxtBox.Text = i_SelectedUser.Name;
               BdayTxtBox.Text = i_SelectedUser.Birthday;
                if (i_SelectedUser.Location != null)
                {
                    LocationTxtBox.Text = i_SelectedUser.Location.Name;
                }
            }
        }

        private void fetchFriendsList()
        {
            LinkedList<User> friends = m_AppLogic.FetchFriends();
            listBoxFriends.DisplayMember = "Name";
            foreach (var friend in friends)
            {
                listBoxFriends.Items.Add(friend);
            }
        }

        private void fetchAlbums()
        {
            new Thread(addAlbums).Start();
            new Thread(setComboBoxOptions).Start();
            comboBoxSort.Enabled = true;
            ButtonFetchAlbums.Enabled = false;
        }

        private void setComboBoxOptions()
        {
            Type type = this.GetType();
            IEnumerable<MethodInfo> info = type.GetRuntimeMethods();
            foreach (MethodInfo method in info)
            {
                Object[] attr = method.GetCustomAttributes(true);
                foreach (var att in attr)
                {
                    if (att is SortingAttribute)
                    {
                        SortingAttribute s = att as SortingAttribute;
                        comboBoxSort.Invoke(new Action(() => comboBoxSort.Items.Add(s)));
                    }
                }
            }
        }

        private void addAlbums()
        {
            Albums = new LinkedList<Album>(m_AppLogic.FetchAlbums());
            foreach (Album album in Albums)
            {
                listBoxAlbums.Invoke(new Action(() =>
                {
                    listBoxAlbums.Items.Add(album);
                    listBoxAlbums.DisplayMember = "Name";
                }));
            }
        }

          private void label2_Click(object sender, EventArgs e)
          {
               label2.BackColor = Color.DarkMagenta;
          }

        [Sorting("Creation Date")]
        private void sortByCreationDate()
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";
            foreach (Album album in Albums.OrderBy(key => key.CreatedTime))
            {
                listBoxAlbums.Invoke(new Action(() => listBoxAlbums.Items.Add(album)));
            }
        }

        [Sorting("Name")]
        private void sortByName()
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";
            foreach (Album album in Albums.OrderBy(key => key.Name))
            {
                listBoxAlbums.Invoke(new Action(() => listBoxAlbums.Items.Add(album)));
            }
        }

        [Sorting("Number Of Photos")]
        private void sortByNumberOfPhotos()
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";
            foreach (Album album in Albums.OrderByDescending(key => key.Count))
            {
                listBoxAlbums.Invoke(new Action(() => listBoxAlbums.Items.Add(album)));
            }
        }
    }
}