using System.Collections.Generic;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace AppLogic
{
    public class ApplicationLogic
    {
        private FriendsAndAlbumsFacade m_Facade;
        public User LoggedInUser { get; set; }
        public LikesPerFriendBuilder LikesPerFriendBuilder { get; set; }
        public FirstLikersBuilder FirstLikersBuilder { get; set; }
        List<IFeatureBuilder> m_FeatureBuilders = new List<IFeatureBuilder>();

        public ApplicationLogic()
        {
            LikesPerFriendBuilder = new LikesPerFriendBuilder();
            FirstLikersBuilder = new FirstLikersBuilder();
            m_FeatureBuilders.Add(LikesPerFriendBuilder);
            m_FeatureBuilders.Add(FirstLikersBuilder);
        }

    public bool LoginSuccess()
        {
            bool loginSucceeded = false;
            LoginResult result = FacebookService.Login("882149751961134",
               "public_profile",
                "user_about_me",
                "user_friends",
                "publish_actions",
                "user_likes",
                "user_photos",
                "user_posts",
                "user_hometown",
                "user_location",
                "user_managed_groups",
                "user_relationships",
                "user_relationship_details",
                "user_religion_politics",
                "user_education_history",
                "user_birthday");

            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                LoggedInUser = result.LoggedInUser;
                foreach (IFeatureBuilder builder in m_FeatureBuilders)
                {
                    builder.LoggedInUser = LoggedInUser;
                }

               m_Facade = new FriendsAndAlbumsFacade(LoggedInUser);
               m_Facade.FetchAlbums();
               m_Facade.FetchFriends();
               loginSucceeded = true;
            }

            return loginSucceeded;
        }

        public void PostStatus(string i_StatusToPost)
        {
               LoggedInUser.PostStatus(i_StatusToPost);
        }

        public LinkedList<Album> FetchAlbums()
        {
            return m_Facade.Albums;
        }

        public LinkedList<User> FetchFriends()
        {
            return m_Facade.Friends;
        }

        public LinkedList<Post> FetchPstsFromUser()
        {
            LinkedList<Post> posts = new LinkedList<Post>();
            foreach (Post post in LoggedInUser.Posts)
            {
                posts.AddLast(post);
            }

            return posts;
        }
    }
}
